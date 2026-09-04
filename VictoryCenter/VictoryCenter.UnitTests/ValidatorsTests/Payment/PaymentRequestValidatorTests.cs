using FluentValidation.TestHelper;
using Microsoft.Extensions.Options;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Public.Payment;
using VictoryCenter.BLL.DTOs.Public.Payment.Common;
using VictoryCenter.BLL.Options.Payment;
using VictoryCenter.BLL.Validators.Payment;

namespace VictoryCenter.UnitTests.ValidatorsTests.Payment;

public class PaymentRequestValidatorTests
{
    private readonly PaymentRequestValidator _validator;

    public PaymentRequestValidatorTests()
    {
        _validator = new PaymentRequestValidator(Options.Create(new WayForPayOptions
        {
            MerchantLogin = "test-login",
            MerchantSecretKey = "test-secret",
            MerchantDomainName = "donate.example.com",
            ApiUrl = "https://secure.wayforpay.com/pay",
            AllowedReturnUrlHosts = ["donate.example.com", "localhost"]
        }));
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

    [Fact]
    public void Validate_ReturnUrlUsesAllowedHost_ShouldNotHaveValidationError()
    {
        var dto = CreateValidRequest() with
        {
            ReturnUrl = "https://donate.example.com/payment/result?source=wayforpay"
        };

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveValidationErrorFor(x => x.ReturnUrl);
    }

    [Fact]
    public void Validate_ReturnUrlUsesAllowedLocalDevelopmentHost_ShouldNotHaveValidationError()
    {
        var dto = CreateValidRequest() with
        {
            ReturnUrl = "http://localhost:3000/payment/result"
        };

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveValidationErrorFor(x => x.ReturnUrl);
    }

    [Theory]
    [InlineData("http://donate.example.com/payment/result")]
    [InlineData("https://evil.example.com/payment/result")]
    [InlineData("https://evil.donate.example.com/payment/result")]
    [InlineData("https://donate.example.com.evil.example/payment/result")]
    [InlineData("https://donate.example.com@evil.example/payment/result")]
    [InlineData("https://donate.example.com:444/payment/result")]
    [InlineData("not-a-url")]
    public void Validate_ReturnUrlIsNotTrusted_ShouldHaveValidationError(string returnUrl)
    {
        var dto = CreateValidRequest() with { ReturnUrl = returnUrl };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.ReturnUrl)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeInAValidFormat(nameof(PaymentRequestDto.ReturnUrl)));
    }

    private static PaymentRequestDto CreateValidRequest()
    {
        return new PaymentRequestDto
        {
            Amount = 100,
            Currency = Currency.UAH,
            PaymentSystem = PaymentSystem.WayForPay
        };
    }
}
