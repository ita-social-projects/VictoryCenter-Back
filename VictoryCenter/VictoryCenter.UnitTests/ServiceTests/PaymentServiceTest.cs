using FluentResults;
using FluentValidation;
using Moq;
using VictoryCenter.BLL.Commands.Public.Payment.Common;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Public.Payment;
using VictoryCenter.BLL.DTOs.Public.Payment.Common;
using VictoryCenter.BLL.Interfaces.PaymentService;
using VictoryCenter.BLL.Services.PaymentService;

namespace VictoryCenter.UnitTests.ServiceTests;

public class PaymentServiceTest
{
    private readonly Mock<IValidator<PaymentRequestDto>> _validatorMock;
    private readonly Mock<IPaymentCommandHandler<PaymentCommand, Result<PaymentResponseDto>>> _handlerMock;
    private readonly Mock<IPaymentFactory> _factoryMock;

    public PaymentServiceTest()
    {
        _validatorMock = new Mock<IValidator<PaymentRequestDto>>();
        _handlerMock = new Mock<IPaymentCommandHandler<PaymentCommand, Result<PaymentResponseDto>>>();
        _factoryMock = new Mock<IPaymentFactory>();
    }

    [Fact]
    public async Task CreatePayment_ValidationFails_ReturnsValidationErrors()
    {
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<PaymentRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult(new[]
            {
                new FluentValidation.Results.ValidationFailure(nameof(PaymentRequestDto.Amount), ErrorMessagesConstants.PropertyIsRequired(nameof(PaymentRequestDto.Amount)))
            }));
        var service = new PaymentService([], _validatorMock.Object);
        var request = new PaymentRequestDto { Amount = 0, Currency = Currency.USD, PaymentSystem = PaymentSystem.WayForPay };

        var result = await service.CreatePayment(request, CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Contains(ErrorMessagesConstants.PropertyIsRequired(nameof(PaymentRequestDto.Amount)), result.Errors.Select(e => e.Message));
        _validatorMock.Verify(x => x.ValidateAsync(It.IsAny<PaymentRequestDto>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreatePayment_NoFactoryMatchesPaymentSystem_ReturnsError()
    {
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<PaymentRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        var service = new PaymentService([], _validatorMock.Object);
        var request = new PaymentRequestDto { Amount = 10, Currency = Currency.USD, PaymentSystem = PaymentSystem.WayForPay };

        var result = await service.CreatePayment(request, CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Contains(PaymentConstants.ChosenPaymentSystemIsNotSupported, result.Errors.Select(e => e.Message));
        _validatorMock.Verify(x => x.ValidateAsync(It.IsAny<PaymentRequestDto>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreatePayment_ValidRequest_CallsFactoryAndHandlerSuccessfully()
    {
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<PaymentRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _handlerMock.Setup(h => h.Handle(It.IsAny<PaymentCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(new PaymentResponseDto { PaymentUrl = "http://test.url" }));
        _factoryMock.Setup(f => f.PaymentSystem).Returns(PaymentSystem.WayForPay);
        _factoryMock.Setup(f => f.GetRequestHandler()).Returns(_handlerMock.Object);
        var service = new PaymentService([_factoryMock.Object], _validatorMock.Object);
        var request = new PaymentRequestDto { Amount = 10, Currency = Currency.USD, PaymentSystem = PaymentSystem.WayForPay };

        var result = await service.CreatePayment(request, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("http://test.url", result.Value.PaymentUrl);
        _validatorMock.Verify(x => x.ValidateAsync(It.IsAny<PaymentRequestDto>(), It.IsAny<CancellationToken>()), Times.Once);
        _handlerMock.Verify(h => h.Handle(It.IsAny<PaymentCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
