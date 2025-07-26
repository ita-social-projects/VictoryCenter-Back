using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using VictoryCenter.BLL.Commands.Payment.Common;
using VictoryCenter.BLL.Commands.Payment.WayForPay;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Payment;
using VictoryCenter.BLL.DTOs.Payment.Common;
using VictoryCenter.BLL.Options.Payment;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Donation;

public class WayForPayPaymentCommandHandlerTests
{
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly Mock<ILogger<WayForPayPaymentCommandHandler>> _loggerMock;
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly IOptions<WayForPayOptions> _options;
    private readonly PaymentRequestDto _basePaymentRequest;
    private readonly PaymentRequestDto _subscriptionPaymentRequest;
    private readonly PaymentCommand _basePaymentCommand;

    public WayForPayPaymentCommandHandlerTests()
    {
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _loggerMock = new Mock<ILogger<WayForPayPaymentCommandHandler>>();
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _options = Options.Create(GetDefaultOptions());

        _basePaymentRequest = new PaymentRequestDto
        {
            Amount = 100,
            Currency = Currency.USD,
            IsSubscription = false,
            PaymentSystem = PaymentSystem.WayForPay
        };

        _subscriptionPaymentRequest = new PaymentRequestDto
        {
            Amount = 100,
            Currency = Currency.USD,
            IsSubscription = true,
            PaymentSystem = PaymentSystem.WayForPay
        };

        _basePaymentCommand = new PaymentCommand(_basePaymentRequest);
        new PaymentCommand(_subscriptionPaymentRequest);
    }

    [Fact]
    public async Task Handle_RedirectResponseWithLocation_ReturnsPaymentUrl()
    {
        var paymentUrl = "https://pay.test/redirect";
        var response = new HttpResponseMessage(HttpStatusCode.Found)
        {
            Headers = { Location = new Uri(paymentUrl) }
        };
        var httpClient = CreateMockHttpClient(response);
        _httpClientFactoryMock.Setup(f => f.CreateClient("Way4PayClient")).Returns(httpClient);
        var handler = new WayForPayPaymentCommandHandler(_options, _httpClientFactoryMock.Object, _loggerMock.Object);

        var result = await handler.Handle(_basePaymentCommand, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(paymentUrl, result.Value.PaymentUrl);
        _httpClientFactoryMock.Verify(x => x.CreateClient("Way4PayClient"), Times.Once);
        _httpMessageHandlerMock.Protected()
            .Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(x => x.Method == HttpMethod.Post),
                ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ValidRequestWithReturnUrl_IncludesPaymentFields()
    {
        var paymentUrl = "https://pay.test/redirect";
        var returnUrl = "https://example.com/return";
        var response = new HttpResponseMessage(HttpStatusCode.Found)
        {
            Headers = { Location = new Uri(paymentUrl) }
        };

        var paymentRequestWithReturnUrl = new PaymentRequestDto
        {
            Amount = 100,
            Currency = Currency.USD,
            IsSubscription = false,
            PaymentSystem = PaymentSystem.WayForPay,
            ReturnUrl = returnUrl
        };
        var commandWithReturnUrl = new PaymentCommand(paymentRequestWithReturnUrl);

        HttpRequestMessage? capturedRequest = null;

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((req, _) => { capturedRequest = req; })
            .ReturnsAsync(response);

        var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _httpClientFactoryMock.Setup(f => f.CreateClient("Way4PayClient")).Returns(httpClient);

        var handler = new WayForPayPaymentCommandHandler(_options, _httpClientFactoryMock.Object, _loggerMock.Object);

        var result = await handler.Handle(commandWithReturnUrl, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(paymentUrl, result.Value.PaymentUrl);
        _httpClientFactoryMock.Verify(x => x.CreateClient("Way4PayClient"), Times.Once);

        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Post, capturedRequest.Method);

        var contentString = await capturedRequest.Content.ReadAsStringAsync();
        var parsed = System.Web.HttpUtility.ParseQueryString(contentString);

        Assert.Equal(_options.Value.MerchantLogin, parsed["merchantAccount"]);
        Assert.Equal(_options.Value.MerchantDomainName, parsed["merchantDomainName"]);
        Assert.NotNull(parsed["orderReference"]);
        Assert.NotNull(parsed["orderDate"]);
        Assert.Equal("100", parsed["amount"]);
        Assert.Equal("USD", parsed["currency"]);
        Assert.Equal(PaymentConstants.ProductName, parsed["productName[]"]);
        Assert.Equal("1", parsed["productCount[]"]);
        Assert.Equal("100", parsed["productPrice[]"]);
        Assert.NotNull(parsed["merchantSignature"]);
        Assert.Equal(returnUrl, parsed["returnUrl"]);
    }

    [Fact]
    public async Task Handle_NoRedirectOrNoLocation_ReturnsFail()
    {
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            ReasonPhrase = "Bad Request"
        };
        var httpClient = CreateMockHttpClient(response);
        _httpClientFactoryMock.Setup(f => f.CreateClient("Way4PayClient")).Returns(httpClient);
        var handler = new WayForPayPaymentCommandHandler(_options, _httpClientFactoryMock.Object, _loggerMock.Object);

        var result = await handler.Handle(_basePaymentCommand, CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal("Bad Request", result.Errors[0].Message);
        _httpClientFactoryMock.Verify(x => x.CreateClient("Way4PayClient"), Times.Once);
        _httpMessageHandlerMock.Protected()
            .Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(x => x.Method == HttpMethod.Post),
                ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task Handle_HttpRequestExceptionThrown_ReturnsFailWithErrorMessage()
    {
        var exceptionMessage = "Network connection failed";
        var httpRequestException = new HttpRequestException(exceptionMessage);

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(httpRequestException);

        var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _httpClientFactoryMock.Setup(f => f.CreateClient("Way4PayClient")).Returns(httpClient);

        var handler = new WayForPayPaymentCommandHandler(_options, _httpClientFactoryMock.Object, _loggerMock.Object);

        var result = await handler.Handle(_basePaymentCommand, CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(PaymentConstants.FailedToCommunicateWithPaymentGateway(exceptionMessage), result.Errors[0].Message);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error occured when processing payment request")),
                httpRequestException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_TaskCancelledExceptionThrown_ReturnsFailWithTimeoutMessage()
    {
        var taskCancelledException = new TaskCanceledException("The request was cancelled or timed out");

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(taskCancelledException);

        var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _httpClientFactoryMock.Setup(f => f.CreateClient("Way4PayClient")).Returns(httpClient);

        var handler = new WayForPayPaymentCommandHandler(_options, _httpClientFactoryMock.Object, _loggerMock.Object);

        var result = await handler.Handle(_basePaymentCommand, CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(PaymentConstants.PaymentRequestWasCancelledOrTimedOut, result.Errors[0].Message);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(PaymentConstants.PaymentRequestWasCancelledOrTimedOut)),
                taskCancelledException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    private WayForPayOptions GetDefaultOptions() => new()
    {
        MerchantLogin = "testLogin",
        MerchantSecretKey = "testSecret",
        MerchantDomainName = "test.domain",
        ApiUrl = "https://api.test/way4pay"
    };

    private HttpClient CreateMockHttpClient(HttpResponseMessage response)
    {
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);
        return new HttpClient(_httpMessageHandlerMock.Object);
    }
}
