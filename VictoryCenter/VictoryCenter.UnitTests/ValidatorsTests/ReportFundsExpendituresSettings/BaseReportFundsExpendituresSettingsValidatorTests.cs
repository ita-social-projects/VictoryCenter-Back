using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresSettings;
using VictoryCenter.BLL.Validators.ReportFundsExpendituresSettings;

namespace VictoryCenter.UnitTests.ValidatorsTests.ReportFundsExpendituresSettings;

public class BaseReportFundsExpendituresSettingsValidatorTests
{
    private readonly BaseReportFundsExpendituresSettingsValidator _validator;

    public BaseReportFundsExpendituresSettingsValidatorTests()
    {
        _validator = new BaseReportFundsExpendituresSettingsValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenDisclaimerTitleIsEmpty(string? disclaimerTitle)
    {
        // Arrange
        var dto = GetValidDto() with { DisclaimerTitle = disclaimerTitle! };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DisclaimerTitle)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(ReportFundsExpendituresSettingsDto.DisclaimerTitle)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDisclaimerTitleIsTooShort()
    {
        // Arrange
        var dto = GetValidDto() with
        {
            DisclaimerTitle = new string('A', ReportFundsExpendituresSettingsConstants.DisclaimerMinLength - 1)
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DisclaimerTitle)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(ReportFundsExpendituresSettingsDto.DisclaimerTitle),
                ReportFundsExpendituresSettingsConstants.DisclaimerMinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDisclaimerTitleExceedsMaxLength()
    {
        // Arrange
        var dto = GetValidDto() with
        {
            DisclaimerTitle = new string('A', ReportFundsExpendituresSettingsConstants.DisclaimerMaxLength + 1)
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DisclaimerTitle)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(ReportFundsExpendituresSettingsDto.DisclaimerTitle),
                ReportFundsExpendituresSettingsConstants.DisclaimerMaxLength));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenExchangeRateIsNotPositive(decimal exchangeRate)
    {
        // Arrange
        var dto = GetValidDto() with { ExchangeRate = exchangeRate };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ExchangeRate)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(
                nameof(ReportFundsExpendituresSettingsDto.ExchangeRate)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenExchangeRateHasInvalidFormat()
    {
        // Arrange
        var dto = GetValidDto() with { ExchangeRate = 1.1234567m };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ExchangeRate)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeInAValidFormat(
                nameof(ReportFundsExpendituresSettingsDto.ExchangeRate),
                ReportFundsExpendituresSettingsConstants.ExchangeRateFormat));
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenDataIsValid()
    {
        // Arrange
        var dto = GetValidDto();

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    private static UpdateReportFundsExpendituresSettingsDto GetValidDto() => new()
    {
        DisclaimerTitle = "Valid disclaimer",
        ExchangeRate = 40.123456m
    };
}
