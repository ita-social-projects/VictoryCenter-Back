using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Payment;
using VictoryCenter.BLL.DTOs.Payment.Donation;
using VictoryCenter.BLL.Validators.Donation;

namespace VictoryCenter.UnitTests.ValidatorsTests.Donation;

public class DonationRequestValidatorTests
{
    private readonly DonationRequestValidator _validator;

    public DonationRequestValidatorTests()
    {
        _validator = new DonationRequestValidator();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validate_AmountIsNotGreaterThanZero_ShouldHaveValidationError(decimal amount)
    {
        var dto = new DonationRequestDto
        {
            Amount = amount,
            Currency = "USD",
            PaymentSystem = PaymentSystem.Way4Pay
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Amount)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeGreaterThan(nameof(DonationRequestDto.Amount), 0));
    }

    [Fact]
    public void Validate_AmountIsGreaterThanZero_ShouldNotHaveValidationError()
    {
        var dto = new DonationRequestDto
        {
            Amount = 10,
            Currency = "USD",
            PaymentSystem = PaymentSystem.Way4Pay
        };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Amount);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_CurrencyIsNullOrEmpty_ShouldHaveValidationError(string currency)
    {
        var dto = new DonationRequestDto
        {
            Amount = 10,
            Currency = currency!,
            PaymentSystem = PaymentSystem.Way4Pay
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Currency)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(DonationRequestDto.Currency)));
    }

    [Theory]
    [InlineData("usd")]
    [InlineData("US")]
    [InlineData("USDD")]
    [InlineData("US1")]
    [InlineData("US$")]
    public void Validate_CurrencyIsInvalidFormat_ShouldHaveValidationError(string currency)
    {
        var dto = new DonationRequestDto
        {
            Amount = 10,
            Currency = currency,
            PaymentSystem = PaymentSystem.Way4Pay
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Currency);
    }

    [Fact]
    public void Validate_CurrencyIsValid_ShouldNotHaveValidationError()
    {
        var dto = new DonationRequestDto
        {
            Amount = 10,
            Currency = "USD",
            PaymentSystem = PaymentSystem.Way4Pay
        };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Currency);
    }

    [Fact]
    public void Validate_PaymentSystemIsNull_ShouldHaveValidationError()
    {
        var dto = new DonationRequestDto
        {
            Amount = 10,
            Currency = "USD",
            PaymentSystem = (PaymentSystem)0
        };
    }

    [Fact]
    public void Validate_AllFieldsValid_ShouldNotHaveAnyValidationErrors()
    {
        var dto = new DonationRequestDto
        {
            Amount = 100,
            Currency = "EUR",
            PaymentSystem = PaymentSystem.Way4Pay
        };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
