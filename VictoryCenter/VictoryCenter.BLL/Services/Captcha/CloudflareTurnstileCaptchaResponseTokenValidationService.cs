using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VictoryCenter.BLL.Errors;
using VictoryCenter.BLL.Interfaces.Captcha;
using VictoryCenter.BLL.Options.Captcha;

namespace VictoryCenter.BLL.Services.Captcha;

public class CloudflareTurnstileCaptchaResponseTokenValidationService : ICaptchaResponseTokenValidationService
{
    private const int MaximumResponseBytes = 16 * 1024;
    private const int MaximumServerErrorStatusCode = 599;
    private const string VerificationFailed = "Cloudflare Turnstile API verification failed.";
    private readonly HttpClient _httpClient;
    private readonly ILogger<CloudflareTurnstileCaptchaResponseTokenValidationService> _logger;
    private readonly CloudflareTurnstileCaptchaOptions _options;
    private readonly TimeProvider _timeProvider;

    public CloudflareTurnstileCaptchaResponseTokenValidationService(
        HttpClient httpClient,
        IOptions<CloudflareTurnstileCaptchaOptions> turnstileCaptchaOptions,
        ILogger<CloudflareTurnstileCaptchaResponseTokenValidationService> logger,
        TimeProvider? timeProvider = null)
    {
        _httpClient = httpClient;
        _logger = logger;
        _options = turnstileCaptchaOptions.Value;
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    public async Task<Result> ValidateTokenAsync(string token, string? remoteIp = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var timeout = TimeSpan.FromMilliseconds(_options.VerificationTimeoutMilliseconds);
        var started = _timeProvider.GetTimestamp();
        using var deadline = new CancellationTokenSource(timeout, _timeProvider);
        using var budget = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, deadline.Token);
        var parameters = new Dictionary<string, string>
        {
            { "secret", _options.SecretKey },
            { "response", token },

            // Keep one key across retries, including ambiguous network failures after token consumption.
            { "idempotency_key", Guid.NewGuid().ToString("D") }
        };
        if (!string.IsNullOrEmpty(remoteIp))
        {
            parameters.Add("remoteip", remoteIp);
        }

        try
        {
            for (var attempt = 1; attempt <= _options.MaxAttempts; attempt++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (_timeProvider.GetElapsedTime(started) >= timeout)
                {
                    return Fail("Verification deadline exceeded");
                }

                var delay = TimeSpan.FromMilliseconds(_options.RetryDelayMilliseconds);
                try
                {
                    using var request = new HttpRequestMessage(HttpMethod.Post, _options.SiteVerifyUrl)
                    {
                        Content = new FormUrlEncodedContent(parameters)
                    };
                    using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, budget.Token);
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await ReadResultAsync(response.Content, budget.Token);
                        return _timeProvider.GetElapsedTime(started) >= timeout ? Fail("Verification deadline exceeded") : result;
                    }

                    if (!IsTransient(response.StatusCode) || attempt == _options.MaxAttempts)
                    {
                        return Fail("HTTP status", (int)response.StatusCode);
                    }

                    delay = response.Headers.RetryAfter?.Delta
                        ?? (response.Headers.RetryAfter?.Date is { } date ? date - _timeProvider.GetUtcNow() : delay);
                    if (delay < TimeSpan.Zero)
                    {
                        delay = TimeSpan.Zero;
                    }
                }
                catch (Exception exception) when (exception is HttpRequestException or IOException)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (attempt == _options.MaxAttempts)
                    {
                        return Fail("Transport failure");
                    }
                }

                // Timer callbacks can be delayed by scheduler load; enforce the remaining budget independently.
                if (delay >= timeout - _timeProvider.GetElapsedTime(started))
                {
                    return Fail("Retry delay exceeds remaining verification budget");
                }

                await Task.Delay(delay, _timeProvider, budget.Token);
            }
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return Fail("Verification deadline exceeded");
        }
        catch (Exception exception) when (exception is InvalidOperationException or UriFormatException)
        {
            return Fail("Invalid verification endpoint or request configuration");
        }

        return Fail("Attempts exhausted");
    }

    private async Task<Result> ReadResultAsync(HttpContent content, CancellationToken cancellationToken)
    {
        if (content.Headers.ContentLength > MaximumResponseBytes)
        {
            return Fail("Response too large");
        }

        await using var stream = await content.ReadAsStreamAsync(cancellationToken);
        var buffer = new byte[MaximumResponseBytes + 1];
        var count = 0;
        while (count < buffer.Length)
        {
            var read = await stream.ReadAsync(buffer.AsMemory(count), cancellationToken);
            if (read == 0)
            {
                break;
            }

            count += read;
        }

        cancellationToken.ThrowIfCancellationRequested();
        if (count > MaximumResponseBytes)
        {
            return Fail("Response too large");
        }

        try
        {
            var result = JsonSerializer.Deserialize<CaptchaVerificationResponse>(buffer.AsSpan(0, count));
            if (result is null)
            {
                return Fail("Empty verification response");
            }

            if (result.Success)
            {
                return Result.Ok();
            }

            var errors = result.ErrorCodes?.Where(code => !string.IsNullOrWhiteSpace(code)).ToList();
            return errors is { Count: > 0 }
                ? Result.Fail(errors)
                : Result.Fail("CAPTCHA token validation failed. No error codes provided.");
        }
        catch (JsonException)
        {
            return Fail("Invalid verification response");
        }
    }

    private Result Fail(string reason, int? statusCode = null)
    {
        // Provider bodies and exception messages may contain secrets; log metadata only.
        _logger.LogWarning("Turnstile verification failed: {Reason}. Status code: {StatusCode}.", reason, statusCode);
        return Result.Fail(new InternalError(VerificationFailed));
    }

    private static bool IsTransient(HttpStatusCode statusCode) =>
        statusCode is HttpStatusCode.RequestTimeout or HttpStatusCode.TooManyRequests
        || (statusCode >= HttpStatusCode.InternalServerError && (int)statusCode <= MaximumServerErrorStatusCode);

    private sealed class CaptchaVerificationResponse
    {
        [JsonRequired]
        [JsonPropertyName("success")]
        public bool Success { get; init; }

        [JsonPropertyName("error-codes")]
        public List<string>? ErrorCodes { get; init; }
    }
}
