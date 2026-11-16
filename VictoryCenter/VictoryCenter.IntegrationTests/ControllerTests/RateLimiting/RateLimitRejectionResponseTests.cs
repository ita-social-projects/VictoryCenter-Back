using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VictoryCenter.WebAPI.Extensions;
using VictoryCenter.WebAPI.Factories;

namespace VictoryCenter.IntegrationTests.ControllerTests.RateLimiting;

public class RateLimitRejectionResponseTests
{
    private const string FixedWindowPath = "fixed-window";
    private const string ConcurrencyPath = "concurrency";
    private const int WindowSeconds = 60;

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task FixedWindowPolicy_LimitExceeded_ShouldReturnProblemDetailsWithRetryAfter(bool useMvcProblemDetailsFactory)
    {
        await using WebApplication app = await CreateApplicationAsync(Task.CompletedTask, useMvcProblemDetailsFactory);
        using var client = CreateClient(app);

        using var permittedResponse = await client.GetAsync(FixedWindowPath);
        using var rejectedResponse = await client.GetAsync(FixedWindowPath);

        Assert.Equal(HttpStatusCode.OK, permittedResponse.StatusCode);
        Assert.Equal(HttpStatusCode.TooManyRequests, rejectedResponse.StatusCode);
        Assert.Equal("application/problem+json", rejectedResponse.Content.Headers.ContentType?.MediaType);

        var retryAfter = rejectedResponse.Headers.RetryAfter?.Delta;
        Assert.NotNull(retryAfter);
        Assert.InRange(retryAfter.Value.TotalSeconds, 1, WindowSeconds);

        var problemDetails = await rejectedResponse.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problemDetails);
        Assert.Equal(StatusCodes.Status429TooManyRequests, problemDetails.Status);
        Assert.Equal(RateLimitRejectionHandler.Title, problemDetails.Title);
        Assert.Contains($"{(int)retryAfter.Value.TotalSeconds} seconds", problemDetails.Detail);
        Assert.True(problemDetails.Extensions.ContainsKey("traceId"));
    }

    [Fact]
    public async Task ConcurrencyPolicy_QueueFull_ShouldReturnProblemDetailsWithoutRetryAfter()
    {
        var gate = new TaskCompletionSource();
        await using WebApplication app = await CreateApplicationAsync(gate.Task, useMvcProblemDetailsFactory: false);
        using var client = CreateClient(app);

        // Permit limit is 1 and queue limit is 0, so the second concurrent request is rejected.
        var blockedRequest = client.GetAsync(ConcurrencyPath);
        using var rejectedResponse = await SendUntilRejectedAsync(client);
        gate.SetResult();
        using var completedResponse = await blockedRequest;

        Assert.Equal(HttpStatusCode.OK, completedResponse.StatusCode);
        Assert.Equal(HttpStatusCode.TooManyRequests, rejectedResponse.StatusCode);
        Assert.Null(rejectedResponse.Headers.RetryAfter);

        var problemDetails = await rejectedResponse.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problemDetails);
        Assert.Equal(StatusCodes.Status429TooManyRequests, problemDetails.Status);
        Assert.Equal(RateLimitRejectionHandler.Title, problemDetails.Title);
    }

    private static async Task<HttpResponseMessage> SendUntilRejectedAsync(HttpClient client)
    {
        // The first request may not have taken the permit yet; retry briefly until it has.
        for (var attempt = 0; attempt < 50; attempt++)
        {
            var response = await client.GetAsync(ConcurrencyPath);
            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                return response;
            }

            response.Dispose();
            await Task.Delay(TimeSpan.FromMilliseconds(100));
        }

        throw new TimeoutException("The concurrency limiter never rejected a request.");
    }

    private static async Task<WebApplication> CreateApplicationAsync(Task gate, bool useMvcProblemDetailsFactory)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = Environments.Development
        });
        builder.WebHost.UseKestrel().UseUrls("http://127.0.0.1:0");
        builder.Logging.ClearProviders();
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["RateLimiting:AdminLogin:PermitLimit"] = "1",
            ["RateLimiting:AdminLogin:WindowSeconds"] = WindowSeconds.ToString(),
            ["RateLimiting:ImageUpload:PermitLimit"] = "1",
            ["RateLimiting:ImageUpload:QueueLimit"] = "0",
        });
        builder.Services.AddRateLimiterConfiguration();

        if (useMvcProblemDetailsFactory)
        {
            builder.Services.AddControllers();
            builder.Services.AddSingleton<ProblemDetailsFactory, CustomProblemDetailsFactory>();
        }

        WebApplication app = builder.Build();

        app.UseRateLimiter();
        app.MapGet($"/{FixedWindowPath}", () => Results.Ok())
            .RequireRateLimiting(RateLimitingPolicyNameConstants.AdminLogin);
        app.MapGet($"/{ConcurrencyPath}", async () =>
        {
            await gate;
            return Results.Ok();
        }).RequireRateLimiting(RateLimitingPolicyNameConstants.ImageUpload);

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
