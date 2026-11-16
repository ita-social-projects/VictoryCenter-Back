using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.WebAPI.ErrorHandling;
using VictoryCenter.WebAPI.Extensions;
using VictoryCenter.WebAPI.Factories;
using VictoryCenter.WebAPI.Middleware;

namespace VictoryCenter.IntegrationTests.MiddlewareTests;

public class ApiExceptionPipelineTests : IAsyncLifetime
{
    private readonly InMemoryLoggerProvider _logs = new();
    private readonly TaskCompletionSource _requestStarted = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly TaskCompletionSource _requestFinished = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private WebApplication _app = null!;
    private HttpClient _client = null!;

    public async Task InitializeAsync()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = Environments.Production });
        builder.WebHost.UseKestrel().UseUrls("http://127.0.0.1:0");
        builder.Logging.ClearProviders();
        builder.Logging.AddProvider(_logs);
        builder.Services.AddControllers().AddApplicationPart(typeof(FakeErrorController).Assembly);
        builder.Services.AddSingleton<ProblemDetailsFactory, CustomProblemDetailsFactory>();
        builder.Services.AddOptions<LogRedactionOptions>();
        builder.Services.AddSingleton<IRequestLogSanitizer, RequestLogSanitizer>();
        builder.Services.AddApiExceptionHandling();
        _app = builder.Build();
        _app.UseExceptionHandler();
        _app.MapControllers();
        _app.MapGet("/test/ok", () => Results.Ok(new { value = "ok" }));
        _app.MapGet("/test/payload", (HttpContext _) => Task.FromException(new BadHttpRequestException("private payload", 413)));
        _app.MapGet("/test/cancellation", (HttpContext _) => Task.FromException(new OperationCanceledException("private cancellation")));
        _app.MapGet("/test/validation/{value}", (string value) => Task.FromException(new ValidationException([new ValidationFailure("value", value)])));
        _app.MapGet("/test/started", async context =>
        {
            await context.Response.WriteAsync("partial response");
            await context.Response.Body.FlushAsync();
            throw new InvalidOperationException("private late failure");
        });
        _app.MapGet("/test/client-cancellation", async context =>
        {
            _requestStarted.TrySetResult();
            try
            {
                await Task.Delay(Timeout.Infinite, context.RequestAborted);
            }
            finally
            {
                _requestFinished.TrySetResult();
            }
        });
        await _app.StartAsync();
        var address = _app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()!.Addresses.Single();
        _client = new HttpClient(new HttpClientHandler { AllowAutoRedirect = false })
        {
            BaseAddress = new Uri(address),
            Timeout = TimeSpan.FromSeconds(10),
        };
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _app.DisposeAsync();
        _logs.Dispose();
    }

    [Theory]
    [InlineData("/api/Test/ThrowException", 500, "Internal Server Error")]
    [InlineData("/api/Test/ThrowValidationException", 400, "Bad Request")]
    [InlineData("/test/payload", 413, "Payload Too Large")]
    [InlineData("/test/cancellation", 500, "Internal Server Error")]
    public async Task Pipeline_ReturnsProblemDetailsAndCorrectLogging(string path, int expectedStatus, string title)
    {
        using var response = await _client.GetAsync(path + "?ticket=secret-ticket");
        var text = await response.Content.ReadAsStringAsync();
        using var body = JsonDocument.Parse(text);

        Assert.Equal(expectedStatus, (int)response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal(expectedStatus, body.RootElement.GetProperty("status").GetInt32());
        Assert.Equal(title, body.RootElement.GetProperty("title").GetString());
        Assert.False(string.IsNullOrWhiteSpace(body.RootElement.GetProperty("traceId").GetString()));
        if (expectedStatus == StatusCodes.Status413PayloadTooLarge)
        {
            Assert.False(body.RootElement.TryGetProperty("type", out _));
        }
        else
        {
            Assert.False(string.IsNullOrWhiteSpace(body.RootElement.GetProperty("type").GetString()));
        }

        Assert.DoesNotContain("private", text);
        Assert.DoesNotContain("Test Exception", text);
        var entries = _logs.Entries.Where(entry => entry.Category == typeof(ApiExceptionHandler).FullName).ToArray();
        if (expectedStatus == 500)
        {
            var log = Assert.Single(entries);
            Assert.Equal(LogLevel.Critical, log.LogLevel);
            Assert.Contains("ticket=[REDACTED]", log.Message);
            Assert.DoesNotContain("secret-ticket", log.Message);
        }
        else
        {
            Assert.Empty(entries);
        }

        Assert.DoesNotContain(_logs.Entries, entry => entry.Category == "Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware" && entry.Exception is not null);
    }

    [Theory]
    [InlineData("application/json")]
    [InlineData("application/problem+json")]
    [InlineData("*/*")]
    [InlineData("text/html")]
    [InlineData("application/xml")]
    public async Task Pipeline_RegardlessOfAccept_ReturnsNonemptyJsonProblem(string accept)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/Test/ThrowValidationException");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(accept));

        using var response = await _client.SendAsync(request);
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal("Name is required", body.RootElement.GetProperty("errors").GetProperty("Name")[0].GetString());
    }

    [Fact]
    public async Task Pipeline_ConcurrentValidationResponses_RemainRequestSpecific()
    {
        var traces = await Task.WhenAll(Enumerable.Range(0, 24).Select(async number =>
        {
            var expected = $"failure-{number}";
            using var response = await _client.GetAsync($"/test/validation/{expected}");
            using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal(expected, json.RootElement.GetProperty("errors").GetProperty("value")[0].GetString());
            return json.RootElement.GetProperty("traceId").GetString();
        }));

        Assert.Equal(24, traces.Distinct().Count());
    }

    [Fact]
    public async Task Pipeline_ResponseAlreadyStarted_DoesNotAppendProblemDetails()
    {
        using var response = await _client.GetAsync("/test/started", HttpCompletionOption.ResponseHeadersRead);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEqual("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        await Assert.ThrowsAsync<HttpRequestException>(() => response.Content.ReadAsStringAsync());
        Assert.DoesNotContain(_logs.Entries, entry => entry.Category == typeof(ApiExceptionHandler).FullName);
    }

    [Fact]
    public async Task Pipeline_ClientDisconnect_DoesNotLogApplicationServerFailure()
    {
        using var cancellation = new CancellationTokenSource();
        var request = _client.GetAsync("/test/client-cancellation", cancellation.Token);
        await _requestStarted.Task.WaitAsync(TimeSpan.FromSeconds(5));
        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => request);
        await _requestFinished.Task.WaitAsync(TimeSpan.FromSeconds(5));
        using var response = await _client.GetAsync("/test/ok");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.DoesNotContain(_logs.Entries, entry => entry.Category == typeof(ApiExceptionHandler).FullName && entry.LogLevel == LogLevel.Critical);
    }

    [Fact]
    public async Task Pipeline_NormalAndNonexceptionResponses_RemainUnchanged()
    {
        using var ok = await _client.GetAsync("/test/ok");
        Assert.Equal(HttpStatusCode.OK, ok.StatusCode);
        Assert.Equal("ok", (await ok.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("value").GetString());
        using var notFound = await _client.GetAsync("/test/not-found");
        Assert.Equal(HttpStatusCode.NotFound, notFound.StatusCode);
        using var bareError = await _client.GetAsync("/api/Test/Get500Response");
        Assert.Equal(HttpStatusCode.InternalServerError, bareError.StatusCode);
        Assert.DoesNotContain(_logs.Entries, entry => entry.Category == typeof(ApiExceptionHandler).FullName);
    }
}
