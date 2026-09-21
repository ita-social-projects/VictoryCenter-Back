using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VictoryCenter.WebAPI.Extensions;

namespace VictoryCenter.IntegrationTests.ControllerTests.Images;

public class ImageUploadRateLimitTests
{
    private const int PermitLimit = 2;
    private const int QueueLimit = 64;
    private const string UploadPath = "upload";

    [Fact]
    public async Task ImageUpload_WhenPermitsAreTaken_ShouldQueueRequestInsteadOfRejectingIt()
    {
        var gate = new TaskCompletionSource();
        await using WebApplication app = await CreateApplicationAsync(gate.Task);
        using var client = CreateClient(app);

        var requests = SendUploadRequests(client, PermitLimit + 1);

        var anyRequestCompleted = Task.WhenAny(requests);
        var firstToFinish = await Task.WhenAny(anyRequestCompleted, Task.Delay(500));

        Assert.NotSame(anyRequestCompleted, firstToFinish);

        gate.SetResult();
        var responses = await Task.WhenAll(requests);

        Assert.All(responses, response => Assert.Equal(HttpStatusCode.OK, response.StatusCode));
        DisposeResponses(responses);
    }

    [Fact]
    public async Task ImageUpload_WhenQueueIsFull_ShouldReturnTooManyRequests()
    {
        var gate = new TaskCompletionSource();
        await using WebApplication app = await CreateApplicationAsync(gate.Task);
        using var client = CreateClient(app);

        var requests = SendUploadRequests(client, PermitLimit + QueueLimit + 1);

        await Task.Delay(500);
        gate.SetResult();
        var responses = await Task.WhenAll(requests);

        Assert.Equal(PermitLimit + QueueLimit, responses.Count(r => r.StatusCode == HttpStatusCode.OK));
        Assert.Single(responses, r => r.StatusCode == HttpStatusCode.TooManyRequests);

        DisposeResponses(responses);
    }

    private static Task<HttpResponseMessage>[] SendUploadRequests(HttpClient client, int count)
    {
        return Enumerable.Range(0, count)
            .Select(_ => client.GetAsync(UploadPath))
            .ToArray();
    }

    private static void DisposeResponses(IEnumerable<HttpResponseMessage> responses)
    {
        foreach (var response in responses)
        {
            response.Dispose();
        }
    }

    private static async Task<WebApplication> CreateApplicationAsync(Task gate)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = Environments.Development
        });
        builder.WebHost.UseKestrel().UseUrls("http://127.0.0.1:0");
        builder.Logging.ClearProviders();
        builder.Services.AddRateLimiterConfiguration();

        WebApplication app = builder.Build();

        app.UseRateLimiter();
        app.MapGet($"/{UploadPath}", async () =>
        {
            await gate;
            return Results.Ok();
        }).RequireRateLimiting(RateLimitingPolicyNameConstants.ImageUpload);

        await app.StartAsync();
        return app;
    }

    private static HttpClient CreateClient(WebApplication app)
    {
        return new HttpClient
        {
            BaseAddress = new Uri(GetApplicationAddress(app)),
            Timeout = TimeSpan.FromSeconds(30)
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
}
