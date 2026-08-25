using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Commands.Admin.Images.Create;
using VictoryCenter.BLL.DTOs.Admin.Images;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.WebAPI.Controllers.Admin;
using VictoryCenter.WebAPI.Extensions;
using VictoryCenter.WebAPI.Middleware;

namespace VictoryCenter.IntegrationTests.ControllerTests.Images;

public class ImageRequestSizeLimitTests
{
    private const string TestAuthenticationScheme = "ImageRequestSizeLimitTest";

    [Fact]
    public async Task CreateImage_RequestBodyExceedsLimit_ShouldReturnPayloadTooLarge()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = Environments.Development
        });
        builder.WebHost.UseKestrel().UseUrls("http://127.0.0.1:0");
        builder.Logging.ClearProviders();
        builder.Services
            .AddAuthentication(TestAuthenticationScheme)
            .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                TestAuthenticationScheme,
                _ => { });
        builder.Services.AddAuthorization();
        builder.Services.AddControllers().AddApplicationPart(typeof(ImageController).Assembly);

        await using WebApplication app = builder.Build();
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        await app.StartAsync();

        string address = GetApplicationAddress(app);
        using var client = new HttpClient { BaseAddress = new Uri(address) };
        var dto = new CreateImageDto
        {
            Base64 = new string('A', (int)ImageConstants.MaxImageUploadRequestSizeInBytes),
            MimeType = ImageMimeTypes.Png
        };
        using var content = new StringContent(
            JsonSerializer.Serialize(dto),
            Encoding.UTF8,
            "application/json");
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/Image")
        {
            Content = content
        };
        request.Headers.ExpectContinue = true;

        HttpResponseMessage response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.RequestEntityTooLarge, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        using JsonDocument responseBody = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(StatusCodes.Status413PayloadTooLarge, responseBody.RootElement.GetProperty("status").GetInt32());
        Assert.Equal("Payload Too Large", responseBody.RootElement.GetProperty("title").GetString());
        Assert.False(responseBody.RootElement.TryGetProperty("Status", out _));
    }

    [Fact]
    public async Task CreateImage_ConcurrentUploadLimitExceeded_ShouldReturnTooManyRequests()
    {
        var twoRequestsStarted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var releaseRequests = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var startedRequestCount = 0;
        var mediatorMock = new Mock<IMediator>();
        mediatorMock
            .Setup(mediator => mediator.Send(
                It.IsAny<CreateImageCommand>(),
                It.IsAny<CancellationToken>()))
            .Returns(async (CreateImageCommand _, CancellationToken cancellationToken) =>
            {
                if (Interlocked.Increment(ref startedRequestCount) == 2)
                {
                    twoRequestsStarted.TrySetResult(true);
                }

                await releaseRequests.Task.WaitAsync(cancellationToken);
                return Result.Fail<ImageDto>("Test request completed.");
            });

        WebApplicationBuilder builder = CreateTestApplicationBuilder();
        builder.Services.AddSingleton(mediatorMock.Object);
        builder.Services.AddRateLimiterConfiguration();

        await using WebApplication app = builder.Build();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseRateLimiter();
        app.MapControllers();
        await app.StartAsync();

        string address = GetApplicationAddress(app);
        using var client = new HttpClient { BaseAddress = new Uri(address) };
        Task<HttpResponseMessage> firstRequest = SendImageRequestAsync(client);
        Task<HttpResponseMessage> secondRequest = SendImageRequestAsync(client);

        try
        {
            await twoRequestsStarted.Task.WaitAsync(TimeSpan.FromSeconds(5));

            using HttpResponseMessage rejectedResponse = await SendImageRequestAsync(client);

            Assert.Equal(HttpStatusCode.TooManyRequests, rejectedResponse.StatusCode);
            mediatorMock.Verify(
                mediator => mediator.Send(
                    It.IsAny<CreateImageCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(2));
        }
        finally
        {
            releaseRequests.TrySetResult(true);
            using HttpResponseMessage firstResponse = await firstRequest;
            using HttpResponseMessage secondResponse = await secondRequest;
        }
    }

    private static WebApplicationBuilder CreateTestApplicationBuilder()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = Environments.Development
        });
        builder.WebHost.UseKestrel().UseUrls("http://127.0.0.1:0");
        builder.Logging.ClearProviders();
        builder.Services
            .AddAuthentication(TestAuthenticationScheme)
            .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                TestAuthenticationScheme,
                _ => { });
        builder.Services.AddAuthorization();
        builder.Services.AddControllers().AddApplicationPart(typeof(ImageController).Assembly);
        return builder;
    }

    private static string GetApplicationAddress(WebApplication app)
    {
        return app.Services
            .GetRequiredService<IServer>()
            .Features
            .Get<IServerAddressesFeature>()!
            .Addresses
            .Single();
    }

    private static async Task<HttpResponseMessage> SendImageRequestAsync(HttpClient client)
    {
        var dto = new CreateImageDto
        {
            Base64 = "AA==",
            MimeType = ImageMimeTypes.Png
        };
        using var content = new StringContent(
            JsonSerializer.Serialize(dto),
            Encoding.UTF8,
            "application/json");
        return await client.PostAsync("api/Image", content);
    }

    private sealed class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var identity = new ClaimsIdentity(
                [new Claim(ClaimTypes.NameIdentifier, "image-request-size-test")],
                TestAuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, TestAuthenticationScheme);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
