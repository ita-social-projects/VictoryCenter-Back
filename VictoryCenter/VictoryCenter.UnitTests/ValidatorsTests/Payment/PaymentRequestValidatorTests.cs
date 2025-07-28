using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Payment;
using VictoryCenter.BLL.DTOs.Payment.Common;
using VictoryCenter.BLL.Validators.Payment;

namespace VictoryCenter.UnitTests.ValidatorsTests.Payment;

public class PaymentRequestValidatorTests
{
    private readonly PaymentRequestValidator _validator;

    public PaymentRequestValidatorTests()
    {
        _validator = new PaymentRequestValidator();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validate_AmountIsNotGreaterThanZero_ShouldHaveValidationError(decimal amount)
    {
        var dto = new PaymentRequestDto
        {
            Amount = amount,
            Currency = Currency.USD,
            PaymentSystem = PaymentSystem.WayForPay
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Amount)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(PaymentRequestDto.Amount)));
    }

    [Fact]
    public void Validate_AmountIsGreaterThanZero_ShouldNotHaveValidationError()
    {
        var dto = new PaymentRequestDto
        {
            Amount = 10,
            Currency = Currency.USD,
            PaymentSystem = PaymentSystem.WayForPay
        };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void Validate_CurrencyIsValid_ShouldNotHaveValidationError()
    {
        var dto = new PaymentRequestDto
        {
            Amount = 10,
            Currency = Currency.USD,
            PaymentSystem = PaymentSystem.WayForPay
        };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Currency);
    }

    [Fact]
    public void Validate_AllFieldsValid_ShouldNotHaveAnyValidationErrors()
    {
        var dto = new PaymentRequestDto
        {
            Amount = 100,
            Currency = Currency.EUR,
            PaymentSystem = PaymentSystem.WayForPay
        };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
