using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using VictoryCenter.BLL.Options.Captcha;
using VictoryCenter.BLL.Services.Captcha;

namespace VictoryCenter.UnitTests.ServiceTests;

public class CloudflareTurnstileCaptchaResponseTokenValidationServiceTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly Mock<ILogger<CloudflareTurnstileCaptchaResponseTokenValidationService>> _loggerMock;
    private readonly CloudflareTurnstileCaptchaResponseTokenValidationService _service;

    public CloudflareTurnstileCaptchaResponseTokenValidationServiceTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _loggerMock = new Mock<ILogger<CloudflareTurnstileCaptchaResponseTokenValidationService>>();

        var options = new CloudflareTurnstileCaptchaOptions
        {
            SiteVerifyUrl = "https://challenges.cloudflare.com/turnstile/v0/siteverify",
            SecretKey = "test-secret"
        };
        var options1 = Options.Create(options);

        _service = new CloudflareTurnstileCaptchaResponseTokenValidationService(
            _httpClient,
            options1,
            _loggerMock.Object);
    }

    [Fact]
    public async Task ValidateTokenAsync_ValidResponse_ReturnsOk()
    {
        var responseJson = JsonSerializer.Serialize(new { success = true });
        SetupHttpResponse(HttpStatusCode.OK, responseJson);

        var result = await _service.ValidateTokenAsync("valid-token");

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task ValidateTokenAsync_ValidResponseWithRemoteIp_ReturnsOk()
    {
        var responseJson = JsonSerializer.Serialize(new { success = true });
        SetupHttpResponse(HttpStatusCode.OK, responseJson);

        var result = await _service.ValidateTokenAsync("valid-token", "127.0.0.1");

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task ValidateTokenAsync_HttpRequestException_ReturnsFail()
    {
        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException());

        var result = await _service.ValidateTokenAsync("token");

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task ValidateTokenAsync_InvalidOperationException_ReturnsFail()
    {
        var badOptions = Options.Create(new CloudflareTurnstileCaptchaOptions { SiteVerifyUrl = "invalid-url", SecretKey = "secret" });
        var service = new CloudflareTurnstileCaptchaResponseTokenValidationService(_httpClient, badOptions, _loggerMock.Object);

        var result = await service.ValidateTokenAsync("token");

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task ValidateTokenAsync_NotSuccessStatusCode_ReturnsFail()
    {
        SetupHttpResponse(HttpStatusCode.BadRequest, "bad request");

        var result = await _service.ValidateTokenAsync("token");

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task ValidateTokenAsync_NullDeserialization_ReturnsFail()
    {
        SetupHttpResponse(HttpStatusCode.OK, "null");

        var result = await _service.ValidateTokenAsync("token");

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task ValidateTokenAsync_DeserializationException_ReturnsFail()
    {
        SetupHttpResponse(HttpStatusCode.OK, "invalid-json");

        var result = await _service.ValidateTokenAsync("token");

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task ValidateTokenAsync_SuccessFalseNoErrorCode_ReturnsFail()
    {
        var responseJson = JsonSerializer.Serialize(new { success = false });
        SetupHttpResponse(HttpStatusCode.OK, responseJson);

        var result = await _service.ValidateTokenAsync("token");

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task ValidateTokenAsync_SuccessFalseWithErrorCodes_ReturnsFail()
    {
        var json = "{\"success\":false,\"error-codes\":[\"invalid-input-response\"]}";
        SetupHttpResponse(HttpStatusCode.OK, json);

        var result = await _service.ValidateTokenAsync("token");

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Message == "invalid-input-response");
    }

    private void SetupHttpResponse(HttpStatusCode statusCode, string content)
    {
        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(content)
            });
    }
}
