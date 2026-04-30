using System.Net.Http.Json;
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
    private readonly HttpClient _httpClient;
    private readonly ILogger<CloudflareTurnstileCaptchaResponseTokenValidationService> _logger;
    private readonly CloudflareTurnstileCaptchaOptions _turnstileCaptchaOptions;

    public CloudflareTurnstileCaptchaResponseTokenValidationService(
        HttpClient httpClient,
        IOptions<CloudflareTurnstileCaptchaOptions> turnstileCaptchaOptions,
        ILogger<CloudflareTurnstileCaptchaResponseTokenValidationService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _turnstileCaptchaOptions = turnstileCaptchaOptions.Value;
    }

    public async Task<Result> ValidateTokenAsync(string token, string? remoteIp = null)
    {
        var parameters = new Dictionary<string, string>
        {
            { "secret", _turnstileCaptchaOptions.SecretKey },
            { "response", token }
        };

        if (!string.IsNullOrEmpty(remoteIp))
        {
            parameters.Add("remoteip", remoteIp);
        }

        var postContent = new FormUrlEncodedContent(parameters);

        var sendTokenVerificationRequestResult = await SendTokenVerificationRequestAsync(postContent);

        if (sendTokenVerificationRequestResult.IsFailed)
        {
            return Result.Fail(sendTokenVerificationRequestResult.Errors);
        }

        var response = sendTokenVerificationRequestResult.Value;

        if (!response.IsSuccessStatusCode)
        {
            const string errorMessage = "Cloudflare Turnstile API verification failed.";
            _logger.LogError(
                errorMessage + " Status code: {StatusCode}. Response: {Response}",
                response.StatusCode,
                await response.Content.ReadAsStringAsync());

            return Result.Fail(new InternalError(errorMessage));
        }

        var deserializedResponseResult = await DeserializeResponseAsync(response);

        if (deserializedResponseResult.IsFailed)
        {
            return Result.Fail(deserializedResponseResult.Errors);
        }

        var deserializedResponse = deserializedResponseResult.Value;

        if (!deserializedResponse.Success)
        {
            if (deserializedResponse.ErrorCodes is null || deserializedResponse.ErrorCodes.Count == 0)
            {
                return Result.Fail("CAPTCHA token validation failed. No error codes provided.");
            }

            return Result.Fail(deserializedResponse.ErrorCodes);
        }

        return Result.Ok();
    }

    private async Task<Result<HttpResponseMessage>> SendTokenVerificationRequestAsync(FormUrlEncodedContent request)
    {
        try
        {
            var response = await _httpClient.PostAsync(_turnstileCaptchaOptions.SiteVerifyUrl, request);

            return Result.Ok(response);
        }
        catch (InvalidOperationException invalidOperationException)
        {
            const string errorMessage = "Invalid Cloudflare Turnstile API endpoint URL format.";
            _logger.LogCritical(invalidOperationException, errorMessage);

            return Result.Fail(new InternalError(errorMessage));
        }
        catch (Exception requestException) when (requestException is HttpRequestException or TaskCanceledException)
        {
            const string errorMessage = "Cloudflare Turnstile API request failed.";
            _logger.LogError(requestException, errorMessage);

            return Result.Fail(new InternalError(errorMessage));
        }
    }

    private async Task<Result<CaptchaVerificationResponse>> DeserializeResponseAsync(HttpResponseMessage response)
    {
        try
        {
            var deserializedResponse = await response.Content.ReadFromJsonAsync<CaptchaVerificationResponse>();

            if (deserializedResponse is null)
            {
                const string errorMessage = "Failed to deserialize Cloudflare Turnstile API response. Result is null.";
                _logger.LogError(errorMessage);

                return Result.Fail(new InternalError(errorMessage));
            }

            return deserializedResponse!;
        }
        catch (Exception exception)
        {
            const string errorMessage = "Failed to deserialize Cloudflare Turnstile API response.";

            _logger.LogError(exception, errorMessage);

            return Result.Fail(new InternalError(errorMessage));
        }
    }

    private class CaptchaVerificationResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; init; }

        [JsonPropertyName("error-codes")]
        public List<string>? ErrorCodes { get; init; }
    }
}
