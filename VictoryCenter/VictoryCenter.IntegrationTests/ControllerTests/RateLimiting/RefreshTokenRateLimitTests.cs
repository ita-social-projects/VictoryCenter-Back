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
using VictoryCenter.BLL.Commands.Admin.Auth.RefreshToken;
using VictoryCenter.BLL.DTOs.Admin.Auth;
using VictoryCenter.WebAPI.Controllers.Admin;
using VictoryCenter.WebAPI.Extensions;

namespace VictoryCenter.IntegrationTests.ControllerTests.RateLimiting;

public class RefreshTokenRateLimitTests
{
    private const int RefreshPermitLimit = 3;
    private const int LoginPermitLimit = 2;
    private const string RefreshPath = "api/auth/refresh-token";
    private const string LoginPath = "api/auth/login";

    [Fact]
    public async Task RefreshToken_RequestLimitExceeded_ShouldReturnTooManyRequests()
    {
        var mediatorMock = CreateMediatorMock(refreshSucceeds: true);
        await using WebApplication app = await CreateApplicationAsync(mediatorMock);
        using var client = CreateClient(app);

        for (var requestNumber = 0; requestNumber < RefreshPermitLimit; requestNumber++)
        {
            using var permittedResponse = await client.PostAsync(RefreshPath, content: null);
            Assert.Equal(HttpStatusCode.OK, permittedResponse.StatusCode);
        }

        using var rejectedResponse = await client.PostAsync(RefreshPath, content: null);

        Assert.Equal(HttpStatusCode.TooManyRequests, rejectedResponse.StatusCode);
        VerifyRefreshHandled(mediatorMock, RefreshPermitLimit);
    }

    [Fact]
    public async Task RefreshToken_FailedAttempts_ShouldCountTowardsLimit()
    {
        var mediatorMock = CreateMediatorMock(refreshSucceeds: false);
        await using WebApplication app = await CreateApplicationAsync(mediatorMock);
        using var client = CreateClient(app);

        for (var requestNumber = 0; requestNumber < RefreshPermitLimit; requestNumber++)
        {
            using var unauthorizedResponse = await client.PostAsync(RefreshPath, content: null);
            Assert.Equal(HttpStatusCode.Unauthorized, unauthorizedResponse.StatusCode);
        }

        using var rejectedResponse = await client.PostAsync(RefreshPath, content: null);

        Assert.Equal(HttpStatusCode.TooManyRequests, rejectedResponse.StatusCode);
        VerifyRefreshHandled(mediatorMock, RefreshPermitLimit);
    }

    [Fact]
    public async Task RefreshToken_LoginLimitExceeded_ShouldNotAffectRefreshLimit()
    {
        var mediatorMock = CreateMediatorMock(refreshSucceeds: true);
        await using WebApplication app = await CreateApplicationAsync(mediatorMock);
        using var client = CreateClient(app);

        for (var requestNumber = 0; requestNumber < LoginPermitLimit; requestNumber++)
        {
            using var loginResponse = await client.PostAsJsonAsync(
                LoginPath,
                new LoginRequestDto("admin@victorycenter.com", "TestPassword123!"));
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        }

        using var lastLoginResponse = await client.PostAsJsonAsync(
            LoginPath,
            new LoginRequestDto("admin@victorycenter.com", "TestPassword123!"));
        using var refreshResponse = await client.PostAsync(RefreshPath, content: null);

        Assert.Equal(HttpStatusCode.TooManyRequests, lastLoginResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, refreshResponse.StatusCode);
    }

    private static Mock<IMediator> CreateMediatorMock(bool refreshSucceeds)
    {
        var mediatorMock = new Mock<IMediator>();
        mediatorMock
            .Setup(mediator => mediator.Send(It.IsAny<RefreshTokenCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(refreshSucceeds
                ? Result.Ok(new AuthResponseDto("access-token"))
                : Result.Fail<AuthResponseDto>("Unauthorized"));
        mediatorMock
            .Setup(mediator => mediator.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(new AuthResponseDto("access-token")));

        return mediatorMock;
    }

    private static void VerifyRefreshHandled(Mock<IMediator> mediatorMock, int expectedCount)
    {
        mediatorMock.Verify(
            mediator => mediator.Send(It.IsAny<RefreshTokenCommand>(), It.IsAny<CancellationToken>()),
            Times.Exactly(expectedCount));
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
            ["RateLimiting:AuthRefresh:PermitLimit"] = RefreshPermitLimit.ToString(),
            ["RateLimiting:AdminLogin:PermitLimit"] = LoginPermitLimit.ToString(),
        });
        builder.Services.AddAuthorization();
        builder.Services.AddControllers().AddApplicationPart(typeof(AuthController).Assembly);
        builder.Services.AddSingleton(mediatorMock.Object);
        builder.Services.AddRateLimiterConfiguration();

        WebApplication app = builder.Build();

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
}
