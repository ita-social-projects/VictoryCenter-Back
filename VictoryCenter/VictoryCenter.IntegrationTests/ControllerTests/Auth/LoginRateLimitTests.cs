using System.Net;
using System.Net.Http.Json;
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
using VictoryCenter.BLL.Commands.Admin.Auth.Login;
using VictoryCenter.BLL.DTOs.Admin.Auth;
using VictoryCenter.WebAPI.Controllers.Admin;
using VictoryCenter.WebAPI.Extensions;

namespace VictoryCenter.IntegrationTests.ControllerTests.Auth;

public class LoginRateLimitTests
{
    private const int PermittedRequestsPerWindow = 5;

    [Fact]
    public async Task Login_RequestLimitExceeded_ShouldReturnTooManyRequests()
    {
        var mediatorMock = CreateMediatorMock();
        await using WebApplication app = await CreateApplicationAsync(mediatorMock);
        using var client = CreateClient(app);

        for (var requestNumber = 0; requestNumber < PermittedRequestsPerWindow; requestNumber++)
        {
            using var permittedResponse = await SendLoginRequestAsync(client);
            Assert.Equal(HttpStatusCode.OK, permittedResponse.StatusCode);
        }

        using var rejectedResponse = await SendLoginRequestAsync(client);

        Assert.Equal(HttpStatusCode.TooManyRequests, rejectedResponse.StatusCode);
        mediatorMock.Verify(
            mediator => mediator.Send(
                It.IsAny<LoginCommand>(),
                It.IsAny<CancellationToken>()),
            Times.Exactly(PermittedRequestsPerWindow));
    }

    [Fact]
    public async Task Login_RequestsFromDifferentForwardedAddresses_ShouldUseSeparateRateLimits()
    {
        const string firstClientAddress = "198.51.100.10";
        const string secondClientAddress = "203.0.113.10";
        var mediatorMock = CreateMediatorMock();
        await using WebApplication app = await CreateApplicationAsync(
            mediatorMock,
            proxyAddress: "172.20.0.2");
        using var client = CreateClient(app);

        for (var requestNumber = 0; requestNumber < PermittedRequestsPerWindow; requestNumber++)
        {
            using var permittedResponse = await SendLoginRequestAsync(client, firstClientAddress);
            Assert.Equal(HttpStatusCode.OK, permittedResponse.StatusCode);
        }

        using var firstClientRejectedResponse = await SendLoginRequestAsync(client, firstClientAddress);
        using var secondClientResponse = await SendLoginRequestAsync(client, secondClientAddress);

        Assert.Equal(HttpStatusCode.TooManyRequests, firstClientRejectedResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, secondClientResponse.StatusCode);
        mediatorMock.Verify(
            mediator => mediator.Send(
                It.IsAny<LoginCommand>(),
                It.IsAny<CancellationToken>()),
            Times.Exactly(PermittedRequestsPerWindow + 1));
    }

    private static Mock<IMediator> CreateMediatorMock()
    {
        var mediatorMock = new Mock<IMediator>();
        mediatorMock
            .Setup(mediator => mediator.Send(
                It.IsAny<LoginCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(new AuthResponseDto("access-token")));

        return mediatorMock;
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
        builder.Services.AddAuthorization();
        builder.Services.AddControllers().AddApplicationPart(typeof(AuthController).Assembly);
        builder.Services.AddSingleton(mediatorMock.Object);
        builder.Services.AddRateLimiterConfiguration();

        if (proxyAddress is not null)
        {
            builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ReverseProxy:KnownNetworks:0"] = "172.16.0.0/12"
            });
            builder.Services.AddForwardedHeadersConfiguration(builder.Configuration);
        }

        WebApplication app = builder.Build();

        if (proxyAddress is not null)
        {
            app.Use((context, next) =>
            {
                context.Connection.RemoteIpAddress = IPAddress.Parse(proxyAddress);
                return next(context);
            });
            app.UseForwardedHeaders();
        }

        app.UseRateLimiter();
        app.MapControllers();
        await app.StartAsync();
        return app;
    }

    private static HttpClient CreateClient(WebApplication app)
    {
        return new HttpClient
        {
            BaseAddress = new Uri(GetApplicationAddress(app))
        };
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

    private static async Task<HttpResponseMessage> SendLoginRequestAsync(
        HttpClient client,
        string? forwardedFor = null)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/Auth/login")
        {
            Content = JsonContent.Create(new LoginRequestDto("admin@victorycenter.com", "TestPassword123!"))
        };

        if (forwardedFor is not null)
        {
            request.Headers.Add("X-Forwarded-For", forwardedFor);
            request.Headers.Add("X-Forwarded-Proto", "https");
        }

        return await client.SendAsync(request);
    }
}
