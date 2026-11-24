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
        await using WebApplication app = await CreateApplicationAsync(new TaskCompletionSource(), Task.CompletedTask, useMvcProblemDetailsFactory);
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
        var permitTaken = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var gate = new TaskCompletionSource();
        await using WebApplication app = await CreateApplicationAsync(permitTaken, gate.Task, useMvcProblemDetailsFactory: false);
        using var client = CreateClient(app);

        // Permit limit is 1 and queue limit is 0: once the first request holds the permit, the next one is rejected.
        var blockedRequest = client.GetAsync(ConcurrencyPath);
        await permitTaken.Task.WaitAsync(TimeSpan.FromSeconds(30));
        using var rejectedResponse = await client.GetAsync(ConcurrencyPath);
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

    private static async Task<WebApplication> CreateApplicationAsync(
        TaskCompletionSource permitTaken,
        Task gate,
        bool useMvcProblemDetailsFactory)
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
            permitTaken.TrySetResult();
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
