using Microsoft.Extensions.Options;
using Moq;
using VictoryCenter.BLL.Commands.Donation.Way4Pay;
using VictoryCenter.BLL.DTOs.Payment;
using VictoryCenter.BLL.Factories.Donation.Implementations;
using VictoryCenter.BLL.Options.Donation;

namespace VictoryCenter.UnitTests.FactoriesTests.DonationFactory;

public class Way4PayDonationFactoryTests
{
    private readonly Way4PayDonationFactory _donationFactory;

    public Way4PayDonationFactoryTests()
    {
        var optionsMock = new Mock<IOptions<Way4PayOptions>>();
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _donationFactory = new Way4PayDonationFactory(optionsMock.Object, httpClientFactoryMock.Object);
    }

    [Fact]
    public void PaymentSystem_ShouldReturn_Way4Pay()
    {
        var paymentSystem = _donationFactory.PaymentSystem;

        Assert.Equal(PaymentSystem.Way4Pay, paymentSystem);
    }

    [Fact]
    public void GetRequestHandler_ShouldReturn_Way4PayDonationCommandHandler()
    {
        var handler = _donationFactory.GetRequestHandler();

        Assert.NotNull(handler);
        Assert.IsType<Way4PayDonationCommandHandler>(handler);
    }
}
