using System.Net;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.PdfReports;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Queries.Public.PdfReports.GetByLanguageId;
using VictoryCenter.BLL.Queries.Admin.PdfReports.GetById;
using VictoryCenter.BLL.Queries.Admin.PdfReports.GetPreviewTicket;
using VictoryCenter.WebAPI.Controllers.Public;
using VictoryCenter.WebAPI.Extensions;

namespace VictoryCenter.IntegrationTests.ControllerTests.RateLimiting;

public class FileDownloadRateLimitTests
{
    // Lower than public-read, so a 429 below proves the file-download policy is applied, not public-read.
    private const int FileDownloadPermitLimit = 2;
    private const int PublicReadPermitLimit = 100;
    private const string PublicFilePath = "api/PdfReports/1/file";
    private const string PreviewPath = "api/PdfReports/preview/report.pdf?ticket=test-ticket";

    [Theory]
    [InlineData(PublicFilePath)]
    [InlineData(PreviewPath)]
    public async Task FileEndpoint_RequestLimitExceeded_ShouldReturnTooManyRequests(string path)
    {
        var mediatorMock = CreateMediatorMock();
        await using WebApplication app = await CreateApplicationAsync(mediatorMock);
        using var client = CreateClient(app);

        for (var requestNumber = 0; requestNumber < FileDownloadPermitLimit; requestNumber++)
        {
            using var permittedResponse = await client.GetAsync(path);
            Assert.Equal(HttpStatusCode.OK, permittedResponse.StatusCode);
            Assert.Equal("application/pdf", permittedResponse.Content.Headers.ContentType?.MediaType);
        }

        using var rejectedResponse = await client.GetAsync(path);

        Assert.Equal(HttpStatusCode.TooManyRequests, rejectedResponse.StatusCode);
        Assert.Equal(FileDownloadPermitLimit, CountHandledFileRequests(mediatorMock));
    }

    [Fact]
    public async Task FileEndpoint_LimitExceeded_ShouldNotAffectPublicReadEndpoints()
    {
        var mediatorMock = CreateMediatorMock();
        await using WebApplication app = await CreateApplicationAsync(mediatorMock);
        using var client = CreateClient(app);

        for (var requestNumber = 0; requestNumber <= FileDownloadPermitLimit; requestNumber++)
        {
            using var fileResponse = await client.GetAsync(PublicFilePath);
        }

        using var listResponse = await client.GetAsync("api/PdfReports/languageId/1");

        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
    }

    private static Mock<IMediator> CreateMediatorMock()
    {
        var mediatorMock = new Mock<IMediator>();
        mediatorMock
            .Setup(mediator => mediator.Send(It.IsAny<GetPdfReportByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => Result.Ok<Stream>(CreatePdfStream()));
        mediatorMock
            .Setup(mediator => mediator.Send(It.IsAny<GetPdfPreviewTicketQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => Result.Ok(new PdfReportFileDto { FileStream = CreatePdfStream(), FileName = "report.pdf" }));
        mediatorMock
            .Setup(mediator => mediator.Send(It.IsAny<GetPublicPdfReportsByLanguageIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(new PaginationResult<PdfReportDto>([], 0)));

        return mediatorMock;
    }

    private static MemoryStream CreatePdfStream() => new("%PDF-1.4"u8.ToArray());

    private static int CountHandledFileRequests(Mock<IMediator> mediatorMock)
    {
        return mediatorMock.Invocations.Count(invocation =>
            invocation.Arguments is [GetPdfReportByIdQuery or GetPdfPreviewTicketQuery, ..]);
    }

    private static async Task<WebApplication> CreateApplicationAsync(Mock<IMediator> mediatorMock)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = Environments.Development
        });
        builder.WebHost.UseKestrel().UseUrls("http://127.0.0.1:0");
        builder.Logging.ClearProviders();
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["RateLimiting:FileDownload:PermitLimit"] = FileDownloadPermitLimit.ToString(),
            ["RateLimiting:PublicRead:PermitLimit"] = PublicReadPermitLimit.ToString(),
        });
        builder.Services.AddAuthorization();
        builder.Services.AddControllers().AddApplicationPart(typeof(PdfReportsController).Assembly);
        builder.Services.AddSingleton(mediatorMock.Object);
        builder.Services.AddRateLimiterConfiguration();

        WebApplication app = builder.Build();

        app.UseRateLimiter();
        app.UseAuthorization();
        app.MapControllers();
        await app.StartAsync();
        return app;
    }

    private static HttpClient CreateClient(WebApplication app)
    {
        var address = app.Services
            .GetRequiredService<IServer>()
            .Features
            .Get<IServerAddressesFeature>()!
            .Addresses
            .Single();

        return new HttpClient
        {
            BaseAddress = new Uri(address),
            Timeout = TimeSpan.FromSeconds(30)
        };
    }
}
