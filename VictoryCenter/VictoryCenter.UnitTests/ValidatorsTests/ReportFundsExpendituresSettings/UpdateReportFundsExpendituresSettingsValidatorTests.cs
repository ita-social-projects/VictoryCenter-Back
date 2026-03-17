using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresSettings.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresSettings;
using VictoryCenter.BLL.Validators.ReportFundsExpendituresSettings;

namespace VictoryCenter.UnitTests.ValidatorsTests.ReportFundsExpendituresSettings;

public class UpdateReportFundsExpendituresSettingsValidatorTests
{
    private readonly UpdateReportFundsExpendituresSettingsValidator _validator;

    public UpdateReportFundsExpendituresSettingsValidatorTests()
    {
        _validator = new UpdateReportFundsExpendituresSettingsValidator(
            new BaseReportFundsExpendituresSettingsValidator());
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDtoIsNull()
    {
        // Arrange
        var command = new UpdateReportFundsExpendituresSettingsCommand(null!);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UpdateReportFundsExpendituresSettingsDto);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenDisclaimerTitleIsEmpty(string? disclaimerTitle)
    {
        // Arrange
        var dto = GetValidDto() with { DisclaimerTitle = disclaimerTitle! };
        var command = new UpdateReportFundsExpendituresSettingsCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UpdateReportFundsExpendituresSettingsDto.DisclaimerTitle)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(ReportFundsExpendituresSettingsDto.DisclaimerTitle)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenExchangeRateHasInvalidFormat()
    {
        // Arrange
        var dto = GetValidDto() with { ExchangeRate = 1.1234567m };
        var command = new UpdateReportFundsExpendituresSettingsCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UpdateReportFundsExpendituresSettingsDto.ExchangeRate)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeInAValidFormat(
                nameof(ReportFundsExpendituresSettingsDto.ExchangeRate),
                ReportFundsExpendituresSettingsConstants.ExchangeRateFormat));
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenDataIsValid()
    {
        // Arrange
        var command = new UpdateReportFundsExpendituresSettingsCommand(GetValidDto());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    private static UpdateReportFundsExpendituresSettingsDto GetValidDto() => new()
    {
        DisclaimerTitle = "Valid disclaimer",
        ExchangeRate = 40.123456m
    };
}
