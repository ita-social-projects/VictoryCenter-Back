using System.Net;
using System.Text;
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
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Queries.Common.Localization.Languages.GetAll;
using VictoryCenter.WebAPI.Controllers.Public.Localization;
using VictoryCenter.WebAPI.Extensions;

namespace VictoryCenter.IntegrationTests.ControllerTests.RateLimiting;

public class PublicReadRateLimitTests
{
    private const int PublicReadPermitLimit = 3;
    private const int ActionPolicyPermitLimit = 5;
    private const string PublicReadPath = "api/LocalizationLanguage";
    private const string TrustedProxyAddress = "172.20.0.2";
    private const string UntrustedProxyAddress = "203.0.113.50";

    [Fact]
    public async Task PublicRead_RequestLimitExceeded_ShouldReturnTooManyRequests()
    {
        var mediatorMock = CreateMediatorMock();
        await using WebApplication app = await CreateApplicationAsync(mediatorMock);
        using var client = CreateClient(app);

        for (var requestNumber = 0; requestNumber < PublicReadPermitLimit; requestNumber++)
        {
            using var permittedResponse = await SendPublicReadRequestAsync(client);
            Assert.Equal(HttpStatusCode.OK, permittedResponse.StatusCode);
        }

        using var rejectedResponse = await SendPublicReadRequestAsync(client);

        Assert.Equal(HttpStatusCode.TooManyRequests, rejectedResponse.StatusCode);
        VerifyQueriesHandled(mediatorMock, PublicReadPermitLimit);
    }

    [Fact]
    public async Task PublicRead_RequestsFromDifferentForwardedAddresses_ShouldUseSeparateRateLimits()
    {
        const string firstClientAddress = "198.51.100.10";
        const string secondClientAddress = "203.0.113.10";
        var mediatorMock = CreateMediatorMock();
        await using WebApplication app = await CreateApplicationAsync(mediatorMock, TrustedProxyAddress);
        using var client = CreateClient(app);

        for (var requestNumber = 0; requestNumber < PublicReadPermitLimit; requestNumber++)
        {
            using var permittedResponse = await SendPublicReadRequestAsync(client, firstClientAddress);
            Assert.Equal(HttpStatusCode.OK, permittedResponse.StatusCode);
        }

        using var firstClientRejectedResponse = await SendPublicReadRequestAsync(client, firstClientAddress);
        using var secondClientResponse = await SendPublicReadRequestAsync(client, secondClientAddress);

        Assert.Equal(HttpStatusCode.TooManyRequests, firstClientRejectedResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, secondClientResponse.StatusCode);
        VerifyQueriesHandled(mediatorMock, PublicReadPermitLimit + 1);
    }

    [Fact]
    public async Task PublicRead_RotatingForwardedAddressesFromUntrustedProxy_ShouldNotBypassRateLimit()
    {
        var mediatorMock = CreateMediatorMock();
        await using WebApplication app = await CreateApplicationAsync(mediatorMock, UntrustedProxyAddress);
        using var client = CreateClient(app);

        for (var requestNumber = 0; requestNumber < PublicReadPermitLimit; requestNumber++)
        {
            using var permittedResponse = await SendPublicReadRequestAsync(client, $"198.51.100.{requestNumber + 1}");
            Assert.Equal(HttpStatusCode.OK, permittedResponse.StatusCode);
        }

        using var rejectedResponse = await SendPublicReadRequestAsync(client, "198.51.100.100");

        Assert.Equal(HttpStatusCode.TooManyRequests, rejectedResponse.StatusCode);
        VerifyQueriesHandled(mediatorMock, PublicReadPermitLimit);
    }

    [Theory]
    [InlineData("api/ContactUs", "application/json", "{}")]
    [InlineData("api/Payments/donate", "application/x-www-form-urlencoded", "")]
    public async Task PublicController_ActionWithOwnPolicy_ShouldUseActionPolicyInsteadOfPublicRead(
        string path,
        string contentType,
        string body)
    {
        await using WebApplication app = await CreateApplicationAsync(CreateMediatorMock());
        using var client = CreateClient(app);

        // The action policy allows more requests than public-read, so a request past the public-read
        // limit is still accepted. The invalid body is fine: rate limiting runs before model validation.
        for (var requestNumber = 0; requestNumber < ActionPolicyPermitLimit; requestNumber++)
        {
            using var permittedResponse = await client.PostAsync(path, new StringContent(body, Encoding.UTF8, contentType));
            Assert.NotEqual(HttpStatusCode.TooManyRequests, permittedResponse.StatusCode);
        }

        using var rejectedResponse = await client.PostAsync(path, new StringContent(body, Encoding.UTF8, contentType));

        Assert.Equal(HttpStatusCode.TooManyRequests, rejectedResponse.StatusCode);
    }

    private static Mock<IMediator> CreateMediatorMock()
    {
        var mediatorMock = new Mock<IMediator>();
        mediatorMock
            .Setup(mediator => mediator.Send(
                It.IsAny<GetAllLocalizationLanguagesQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok<IEnumerable<LocalizationLanguageDto>>([]));

        return mediatorMock;
    }

    private static void VerifyQueriesHandled(Mock<IMediator> mediatorMock, int expectedCount)
    {
        mediatorMock.Verify(
            mediator => mediator.Send(
                It.IsAny<GetAllLocalizationLanguagesQuery>(),
                It.IsAny<CancellationToken>()),
            Times.Exactly(expectedCount));
    }

    private static async Task<WebApplication> CreateApplicationAsync(
        Mock<IMediator> mediatorMock,
        string? proxyAddress = null)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = Environments.Development
        });
        builder.WebHost.UseKestrel().UseUrls("http://127.0.0.1:0");
        builder.Logging.ClearProviders();
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["RateLimiting:PublicRead:PermitLimit"] = PublicReadPermitLimit.ToString(),
            ["RateLimiting:SubmitContactUsForm:PermitLimit"] = ActionPolicyPermitLimit.ToString(),
            ["RateLimiting:InitiateDonation:PermitLimit"] = ActionPolicyPermitLimit.ToString(),
            ["ReverseProxy:KnownNetworks:0"] = "172.16.0.0/12",
        });
        builder.Services.AddControllers().AddApplicationPart(typeof(LocalizationLanguageController).Assembly);
        builder.Services.AddSingleton(mediatorMock.Object);
        builder.Services.AddSingleton(Mock.Of<VictoryCenter.BLL.Interfaces.PaymentService.IPaymentService>());
        builder.Services.AddRateLimiterConfiguration();
        builder.Services.AddForwardedHeadersConfiguration(builder.Configuration);

        WebApplication app = builder.Build();

        if (proxyAddress is not null)
        {
            app.Use((context, next) =>
            {
                context.Connection.RemoteIpAddress = IPAddress.Parse(proxyAddress);
                return next(context);
            });
        }

        app.UseForwardedHeaders();
        app.UseRateLimiter();
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

    private static Task<HttpResponseMessage> SendPublicReadRequestAsync(HttpClient client, string? forwardedFor = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, PublicReadPath);
        if (forwardedFor is not null)
        {
            request.Headers.Add("X-Forwarded-For", forwardedFor);
        }

        return client.SendAsync(request);
    }
}
