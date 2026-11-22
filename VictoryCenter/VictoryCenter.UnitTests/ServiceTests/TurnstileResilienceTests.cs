using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using VictoryCenter.BLL.Options.Captcha;
using VictoryCenter.BLL.Services.Captcha;

namespace VictoryCenter.UnitTests.ServiceTests;

public class TurnstileResilienceTests
{
    [Theory]
    [InlineData(408)]
    [InlineData(429)]
    [InlineData(500)]
    [InlineData(502)]
    [InlineData(503)]
    public async Task TransientStatus_RetriesWithStableKeyAndFreshRequests(int status)
    {
        var forms = new List<string>();
        var requests = new List<HttpRequestMessage>();
        using var handler = new StubHandler(async (request, cancellation) =>
        {
            requests.Add(request);
            forms.Add(await request.Content!.ReadAsStringAsync(cancellation));
            return Reply(forms.Count == 1 ? status : 200, "{\"success\":true}");
        });
        using var client = new HttpClient(handler);
        var service = Create(client);

        var result = await service.ValidateTokenAsync("token + &", "127.0.0.1");

        Assert.True(result.IsSuccess);
        Assert.Equal(2, forms.Count);
        Assert.Equal(forms[0], forms[1]);
        Assert.NotSame(requests[0], requests[1]);
        var form = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(forms[0]);
        Assert.Equal("token + &", form["response"]);
        Assert.Equal("127.0.0.1", form["remoteip"]);
        Assert.Equal("test-secret", form["secret"]);
        Assert.True(Guid.TryParse(form["idempotency_key"], out _));
    }

    [Theory]
    [InlineData(301)]
    [InlineData(400)]
    [InlineData(401)]
    [InlineData(403)]
    public async Task PermanentHttpFailure_IsNotRetried(int status)
    {
        var calls = 0;
        using var handler = new StubHandler((_, _) =>
        {
            calls++;
            return Task.FromResult(Reply(status, "sensitive provider body"));
        });
        using var client = new HttpClient(handler);
        Assert.True((await Create(client).ValidateTokenAsync("token")).IsFailed);
        Assert.Equal(1, calls);
    }

    [Theory]
    [InlineData("")]
    [InlineData("null")]
    [InlineData("{}")]
    [InlineData("not json")]
    [InlineData("{\"success\":\"true\"}")]
    [InlineData("{\"success\":false}")]
    [InlineData("{\"success\":false,\"error-codes\":[\"timeout-or-duplicate\"]}")]
    public async Task InvalidOrRejectedResponse_FailsWithoutRetry(string body)
    {
        var calls = 0;
        using var handler = new StubHandler((_, _) =>
        {
            calls++;
            return Task.FromResult(Reply(200, body));
        });
        using var client = new HttpClient(handler);
        Assert.True((await Create(client).ValidateTokenAsync("token")).IsFailed);
        Assert.Equal(1, calls);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task TransientExhaustion_StopsAfterThreeAttempts(bool networkFailure)
    {
        var calls = 0;
        using var handler = new StubHandler((_, _) =>
        {
            calls++;
            return networkFailure ? throw new HttpRequestException("private details") : Task.FromResult(Reply(503, ""));
        });
        using var client = new HttpClient(handler);
        var result = await Create(client).ValidateTokenAsync("token");
        Assert.True(result.IsFailed);
        Assert.Equal(3, calls);
        Assert.DoesNotContain("private details", result.Errors[0].Message);
    }

    [Fact]
    public async Task TransportFailure_Recovers()
    {
        var calls = 0;
        using var handler = new StubHandler((_, _) => ++calls == 1
            ? throw new HttpRequestException()
            : Task.FromResult(Reply(200, "{\"success\":true}")));
        using var client = new HttpClient(handler);
        Assert.True((await Create(client).ValidateTokenAsync("token")).IsSuccess);
        Assert.Equal(2, calls);
    }

    [Fact]
    public async Task CallerCancellation_PropagatesWithoutRetry()
    {
        using var cancellation = new CancellationTokenSource();
        var calls = 0;
        using var handler = new StubHandler(async (_, token) =>
        {
            calls++;
            cancellation.Cancel();
            await Task.Delay(Timeout.Infinite, token);
            return Reply(200, "");
        });
        using var client = new HttpClient(handler);
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => Create(client).ValidateTokenAsync("token", cancellationToken: cancellation.Token));
        Assert.Equal(1, calls);
    }

    [Fact]
    public async Task CancellationDuringRetryDelay_DoesNotSendAnotherAttempt()
    {
        using var cancellation = new CancellationTokenSource();
        var calls = 0;
        using var handler = new StubHandler((_, _) =>
        {
            calls++;
            cancellation.Cancel();
            return Task.FromResult(Reply(503, ""));
        });
        using var client = new HttpClient(handler);
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => Create(client).ValidateTokenAsync("token", cancellationToken: cancellation.Token));
        Assert.Equal(1, calls);
    }

    [Fact]
    public async Task RetriedResponse_IsDisposedBeforeNextAttempt()
    {
        using var stream = new TrackingStream(Encoding.UTF8.GetBytes("private response"), true);
        var calls = 0;
        using var handler = new StubHandler((_, _) =>
        {
            if (++calls == 1)
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable) { Content = new StreamContent(stream) });
            }

            Assert.True(stream.Disposed);
            return Task.FromResult(Reply(200, "{\"success\":true}"));
        });
        using var client = new HttpClient(handler);
        Assert.True((await Create(client).ValidateTokenAsync("token")).IsSuccess);
    }

    [Fact]
    public async Task AlreadyCancelled_DoesNotSendRequest()
    {
        using var handler = new StubHandler((_, _) => throw new InvalidOperationException("Must not be called"));
        using var client = new HttpClient(handler);
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => Create(client).ValidateTokenAsync("token", cancellationToken: new CancellationToken(true)));
    }

    [Fact]
    public async Task Deadline_CancelsHangingRequest()
    {
        var calls = 0;
        using var handler = new StubHandler(async (_, token) =>
        {
            calls++;
            await Task.Delay(Timeout.Infinite, token);
            return Reply(200, "");
        });
        using var client = new HttpClient(handler);
        Assert.True((await Create(client, timeout: 100).ValidateTokenAsync("token").WaitAsync(TimeSpan.FromSeconds(5))).IsFailed);
        Assert.Equal(1, calls);
    }

    [Fact]
    public async Task RetryAfterBeyondBudget_DoesNotRetryEarly()
    {
        var calls = 0;
        using var handler = new StubHandler((_, _) =>
        {
            calls++;
            var response = Reply(429, "");
            response.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromMinutes(1));
            return Task.FromResult(response);
        });
        using var client = new HttpClient(handler);
        Assert.True((await Create(client).ValidateTokenAsync("token")).IsFailed);
        Assert.Equal(1, calls);
    }

    [Fact]
    public async Task RetryDelay_IsInsideTotalBudget()
    {
        var calls = 0;
        using var handler = new StubHandler(async (_, cancellation) =>
        {
            calls++;
            await Task.Delay(100, cancellation);
            var response = Reply(503, "");
            response.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromSeconds(1));
            return response;
        });
        using var client = new HttpClient(handler);
        Assert.True((await Create(client, timeout: 1000).ValidateTokenAsync("token").WaitAsync(TimeSpan.FromSeconds(5))).IsFailed);
        Assert.Equal(1, calls);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task OversizedResponse_FailsAndDisposesStream(bool knownLength)
    {
        using var stream = new TrackingStream(Encoding.UTF8.GetBytes(new string('x', 32 * 1024)), knownLength);
        using var handler = new StubHandler((_, _) =>
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(stream) };
            response.Content.Headers.ContentLength = knownLength ? 32 * 1024 : null;
            return Task.FromResult(response);
        });
        using var client = new HttpClient(handler);
        Assert.True((await Create(client).ValidateTokenAsync("token")).IsFailed);
        Assert.True(stream.Disposed);
    }

    [Theory]
    [InlineData(503)]
    [InlineData(429)]
    public async Task RealHttpServer_RetriesAndParsesResponse(int transientStatus)
    {
        var builder = WebApplication.CreateSlimBuilder();
        builder.Logging.ClearProviders();
        builder.WebHost.ConfigureKestrel(options => options.Listen(IPAddress.Loopback, 0));
        await using var app = builder.Build();
        var forms = new List<string>();
        app.MapPost("/siteverify", async (HttpContext context) =>
        {
            var form = await context.Request.ReadFormAsync(context.RequestAborted);
            forms.Add(form["idempotency_key"].ToString());
            Assert.Equal("token", form["response"]);
            context.Response.StatusCode = forms.Count == 1 ? transientStatus : 200;
            await context.Response.WriteAsync("{\"success\":true}");
        });
        await app.StartAsync();
        var address = app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()!.Addresses.Single();
        using var client = new HttpClient();
        var result = await Create(client, url: address + "/siteverify").ValidateTokenAsync("token");
        Assert.True(result.IsSuccess);
        Assert.Equal(2, forms.Count);
        Assert.Equal(forms[0], forms[1]);
        await app.StopAsync();
    }

    [Fact]
    public async Task RealHttpServer_DeadlineAlsoBoundsResponseBodyReading()
    {
        var builder = WebApplication.CreateSlimBuilder();
        builder.Logging.ClearProviders();
        builder.WebHost.ConfigureKestrel(options => options.Listen(IPAddress.Loopback, 0));
        await using var app = builder.Build();
        var startedReading = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        app.MapPost("/siteverify", async (HttpContext context) =>
        {
            await context.Response.StartAsync(context.RequestAborted);
            await context.Response.WriteAsync("{", context.RequestAborted);
            await context.Response.Body.FlushAsync(context.RequestAborted);
            startedReading.TrySetResult();
            try { await Task.Delay(Timeout.Infinite, context.RequestAborted); }
            catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested) { }
        });
        await app.StartAsync();
        var address = app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()!.Addresses.Single();
        using var client = new HttpClient();
        var validation = Create(client, timeout: 1000, url: address + "/siteverify").ValidateTokenAsync("token");
        await startedReading.Task.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.True((await validation.WaitAsync(TimeSpan.FromSeconds(5))).IsFailed);
        await app.StopAsync();
    }

    [Fact]
    public async Task SeparateValidations_UseDifferentKeysAndDisposeResponses()
    {
        var keys = new List<string>();
        var streams = new List<TrackingStream>();
        using var handler = new StubHandler(async (request, cancellation) =>
        {
            var form = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(await request.Content!.ReadAsStringAsync(cancellation));
            keys.Add(form["idempotency_key"].ToString());
            var stream = new TrackingStream(Encoding.UTF8.GetBytes("{\"success\":true}"), true);
            streams.Add(stream);
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(stream) };
        });
        using var client = new HttpClient(handler);
        var service = Create(client);
        Assert.True((await service.ValidateTokenAsync("token")).IsSuccess);
        Assert.True((await service.ValidateTokenAsync("token")).IsSuccess);
        Assert.NotEqual(keys[0], keys[1]);
        Assert.All(streams, stream => Assert.True(stream.Disposed));
    }

    private static CloudflareTurnstileCaptchaResponseTokenValidationService Create(HttpClient client, int timeout = 10000, string url = "https://turnstile.test/siteverify") =>
        new(client, Options.Create(new CloudflareTurnstileCaptchaOptions
        {
            SecretKey = "test-secret", SiteVerifyUrl = url, VerificationTimeoutMilliseconds = timeout, RetryDelayMilliseconds = 0
        }), NullLogger<CloudflareTurnstileCaptchaResponseTokenValidationService>.Instance);

    private static HttpResponseMessage Reply(int status, string body) => new((HttpStatusCode)status) { Content = new StringContent(body) };

    private sealed class StubHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> send) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => send(request, cancellationToken);
    }

    private sealed class TrackingStream(byte[] bytes, bool knownLength) : MemoryStream(bytes)
    {
        public bool Disposed { get; private set; }

        public override bool CanSeek => knownLength && base.CanSeek;

        protected override void Dispose(bool disposing)
        {
            Disposed = true;
            base.Dispose(disposing);
        }
    }
}
