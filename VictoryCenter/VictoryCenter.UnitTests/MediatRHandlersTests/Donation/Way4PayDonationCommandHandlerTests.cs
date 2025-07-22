using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using VictoryCenter.BLL.Commands.Donation.Common;
using VictoryCenter.BLL.Commands.Donation.Way4Pay;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Payment;
using VictoryCenter.BLL.DTOs.Payment.Donation;
using VictoryCenter.BLL.Options.Donation;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Donation;

public class Way4PayDonationCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnPaymentUrl_WhenRedirectResponseWithLocation()
    {
        var options = Options.Create(GetDefaultOptions());
        var paymentUrl = "https://pay.test/redirect";
        var response = new HttpResponseMessage(HttpStatusCode.Found)
        {
            Headers = { Location = new Uri(paymentUrl) }
        };
        var (httpClient, mockHttpMessageHandler) = CreateMockHttpClient(response);
        var httpClientFactory = new Mock<IHttpClientFactory>();
        httpClientFactory.Setup(f => f.CreateClient("Way4PayClient")).Returns(httpClient);
        var logger = new Mock<ILogger<Way4PayDonationCommandHandler>>();
        var handler = new Way4PayDonationCommandHandler(options, httpClientFactory.Object, logger.Object);
        var command = new DonationCommand(new DonationRequestDto
        {
            Amount = 100,
            Currency = "USD",
            IsSubscription = false,
            PaymentSystem = PaymentSystem.Way4Pay
        });

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(paymentUrl, result.Value.PaymentUrl);
        httpClientFactory.Verify(x => x.CreateClient("Way4PayClient"), Times.Once);
        mockHttpMessageHandler.Protected()
            .Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(x => x.Method == HttpMethod.Post),
                ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenNoRedirectOrNoLocation()
    {
        var options = Options.Create(GetDefaultOptions());
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            ReasonPhrase = "Bad Request"
        };
        var (httpClient, mockHttpMessageHandler) = CreateMockHttpClient(response);
        var httpClientFactory = new Mock<IHttpClientFactory>();
        httpClientFactory.Setup(f => f.CreateClient("Way4PayClient")).Returns(httpClient);
        var logger = new Mock<ILogger<Way4PayDonationCommandHandler>>();
        var handler = new Way4PayDonationCommandHandler(options, httpClientFactory.Object, logger.Object);
        var command = new DonationCommand(new DonationRequestDto
        {
            Amount = 100,
            Currency = "USD",
            IsSubscription = false,
            PaymentSystem = PaymentSystem.Way4Pay
        });

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal("Bad Request", result.Errors[0].Message);
        httpClientFactory.Verify(x => x.CreateClient("Way4PayClient"), Times.Once);
        mockHttpMessageHandler.Protected()
            .Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(x => x.Method == HttpMethod.Post),
                ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldIncludeSubscriptionFields_WhenIsSubscriptionTrue()
    {
        var options = Options.Create(GetDefaultOptions());
        var paymentUrl = "https://pay.test/redirect";
        var response = new HttpResponseMessage(HttpStatusCode.Found)
        {
            Headers = { Location = new Uri(paymentUrl) }
        };

        var (httpClient, mockHttpMessageHandler) = CreateMockHttpClient(response);
        var httpClientFactory = new Mock<IHttpClientFactory>();
        httpClientFactory.Setup(f => f.CreateClient("Way4PayClient")).Returns(httpClient);

        var logger = new Mock<ILogger<Way4PayDonationCommandHandler>>();
        var handler = new Way4PayDonationCommandHandler(options, httpClientFactory.Object, logger.Object);

        var command = new DonationCommand(new DonationRequestDto
        {
            Amount = 100,
            Currency = "USD",
            IsSubscription = true,
            PaymentSystem = PaymentSystem.Way4Pay
        });

        HttpRequestMessage? capturedRequest = null;

        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((req, _) => { capturedRequest = req; })
            .ReturnsAsync(response);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(paymentUrl, result.Value.PaymentUrl);
        httpClientFactory.Verify(x => x.CreateClient("Way4PayClient"), Times.Once);

        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Post, capturedRequest.Method);

        var contentString = await capturedRequest.Content.ReadAsStringAsync();
        var parsed = System.Web.HttpUtility.ParseQueryString(contentString);

        Assert.Equal("1", parsed["regularOn"]);
        Assert.Equal("100", parsed["regularAmount"]);
        Assert.Equal(PaymentConstants.RegularPaymentMode, parsed["regularMode"]);
        Assert.Equal(PaymentConstants.RegularPaymentBehaviour, parsed["regularBehavior"]);
        Assert.Equal(PaymentConstants.RegularPaymentCount, parsed["regularCount"]);
    }

    private Way4PayOptions GetDefaultOptions() => new()
    {
        MerchantLogin = "testLogin",
        MerchantSecretKey = "testSecret",
        MerchantDomainName = "test.domain",
        ApiUrl = "https://api.test/way4pay"
    };

    private static (HttpClient HttpClient, Mock<HttpMessageHandler> MockHttpMessageHandler) CreateMockHttpClient(HttpResponseMessage response)
    {
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);
        return (new HttpClient(handler.Object), handler);
    }
}
