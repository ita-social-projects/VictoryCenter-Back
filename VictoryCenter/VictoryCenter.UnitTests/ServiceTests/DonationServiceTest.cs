using FluentResults;
using FluentValidation;
using Moq;
using VictoryCenter.BLL.Commands.Donation.Common;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Payment;
using VictoryCenter.BLL.DTOs.Payment.Donation;
using VictoryCenter.BLL.Factories.Donation.Interfaces;
using VictoryCenter.BLL.Services.PaymentService;

namespace VictoryCenter.UnitTests.ServiceTests;

public class DonationServiceTest
{
    [Fact]
    public async Task CreateDonation_ShouldReturnValidationErrors_WhenValidationFails()
    {
        var validatorMock = new Mock<IValidator<DonationRequestDto>>();
        validatorMock.Setup(v => v.ValidateAsync(It.IsAny<DonationRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult(new[]
            {
                new FluentValidation.Results.ValidationFailure("Amount", "Amount is required")
            }));
        var service = new DonationService([], validatorMock.Object);
        var request = new DonationRequestDto { Amount = 0, Currency = "USD", PaymentSystem = PaymentSystem.Way4Pay };

        var result = await service.CreateDonation(request, CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Contains("Amount is required", result.Errors.Select(e => e.Message));
        validatorMock.Verify(x => x.ValidateAsync(It.IsAny<DonationRequestDto>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateDonation_ShouldReturnError_WhenNoFactoryMatchesPaymentSystem()
    {
        var validatorMock = new Mock<IValidator<DonationRequestDto>>();
        validatorMock.Setup(v => v.ValidateAsync(It.IsAny<DonationRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        var service = new DonationService([], validatorMock.Object);
        var request = new DonationRequestDto { Amount = 10, Currency = "USD", PaymentSystem = PaymentSystem.Way4Pay };

        var result = await service.CreateDonation(request, CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Contains(PaymentConstants.ChosenPaymentSystemIsNotSupported, result.Errors.Select(e => e.Message));
        validatorMock.Verify(x => x.ValidateAsync(It.IsAny<DonationRequestDto>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateDonation_ShouldCallFactoryAndHandler_WhenValidRequest()
    {
        var validatorMock = new Mock<IValidator<DonationRequestDto>>();
        validatorMock.Setup(v => v.ValidateAsync(It.IsAny<DonationRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        var handlerMock = new Mock<IDonationCommandHandler<DonationCommand, Result<DonationResponseDto>>>();
        handlerMock.Setup(h => h.Handle(It.IsAny<DonationCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(new DonationResponseDto { PaymentUrl = "http://test.url" }));
        var factoryMock = new Mock<IDonationFactory>();
        factoryMock.Setup(f => f.PaymentSystem).Returns(PaymentSystem.Way4Pay);
        factoryMock.Setup(f => f.GetRequestHandler()).Returns(handlerMock.Object);
        var service = new DonationService([factoryMock.Object], validatorMock.Object);
        var request = new DonationRequestDto { Amount = 10, Currency = "USD", PaymentSystem = PaymentSystem.Way4Pay };

        var result = await service.CreateDonation(request, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("http://test.url", result.Value.PaymentUrl);
        validatorMock.Verify(x => x.ValidateAsync(It.IsAny<DonationRequestDto>(), It.IsAny<CancellationToken>()), Times.Once);
        handlerMock.Verify(h => h.Handle(It.IsAny<DonationCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
