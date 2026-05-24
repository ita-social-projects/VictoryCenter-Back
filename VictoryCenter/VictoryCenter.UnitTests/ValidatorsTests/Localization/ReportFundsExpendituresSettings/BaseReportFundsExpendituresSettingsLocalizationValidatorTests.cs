using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.ReportFundsExpendituresSettings;
using VictoryCenter.BLL.Validators.Localization.ReportFundsExpendituresSettings;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.ReportFundsExpendituresSettings;

public class BaseReportFundsExpendituresSettingsLocalizationValidatorTests
{
    private readonly BaseReportFundsExpendituresSettingsLocalizationValidator _validator;

    public BaseReportFundsExpendituresSettingsLocalizationValidatorTests()
    {
        _validator = new BaseReportFundsExpendituresSettingsLocalizationValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenDisclaimerTitle_IsNullOrEmpty(string? disclaimerTitle)
    {
        var model = new UpdateReportFundsExpendituresSettingsLocalizationDto { DisclaimerTitle = disclaimerTitle! };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DisclaimerTitle);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDisclaimerTitle_IsTooShort()
    {
        var model = new UpdateReportFundsExpendituresSettingsLocalizationDto
        {
            DisclaimerTitle = new string('a', ReportFundsExpendituresSettingsConstants.DisclaimerMinLength - 1)
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DisclaimerTitle);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDisclaimerTitle_IsTooLong()
    {
        var model = new UpdateReportFundsExpendituresSettingsLocalizationDto
        {
            DisclaimerTitle = new string('a', ReportFundsExpendituresSettingsConstants.DisclaimerMaxLength + 1)
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DisclaimerTitle);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenModel_IsValid()
    {
        var model = new UpdateReportFundsExpendituresSettingsLocalizationDto
        {
            DisclaimerTitle = "Valid disclaimer text"
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.DisclaimerTitle);
    }
}
