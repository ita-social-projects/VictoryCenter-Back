using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using VictoryCenter.BLL.Commands.Payment.WayForPay;
using VictoryCenter.BLL.DTOs.Payment;
using VictoryCenter.BLL.Factories.Payment.Implementations;
using VictoryCenter.BLL.Options.Payment;

namespace VictoryCenter.UnitTests.FactoriesTests.PaymentFactory;

public class WayForPayPaymentFactoryTests
{
    private readonly Mock<IOptions<WayForPayOptions>> _optionsMock;
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly Mock<ILogger<WayForPayPaymentCommandHandler>> _loggerMock;
    private readonly WayForPayPaymentFactory _paymentFactory;

    public WayForPayPaymentFactoryTests()
    {
        _optionsMock = new Mock<IOptions<WayForPayOptions>>();
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _loggerMock = new Mock<ILogger<WayForPayPaymentCommandHandler>>();
        _paymentFactory = new WayForPayPaymentFactory(_optionsMock.Object, _httpClientFactoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public void PaymentSystem_Called_ReturnsWay4Pay()
    {
        var paymentSystem = _paymentFactory.PaymentSystem;

        Assert.Equal(PaymentSystem.WayForPay, paymentSystem);
    }

    [Fact]
    public void GetRequestHandler_Called_ReturnsWay4PayDonationCommandHandler()
    {
        var handler = _paymentFactory.GetRequestHandler();

        Assert.NotNull(handler);
        Assert.IsType<WayForPayPaymentCommandHandler>(handler);
    }
}
