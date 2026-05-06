using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.ReportFundsExpendituresCategories;
using VictoryCenter.BLL.Validators.Localization.ReportFundsExpendituresCategories;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.ReportFundsExpendituresCategories;

public class BaseReportFundsExpendituresCategoryLocalizationValidatorTests
{
    private readonly BaseReportFundsExpendituresCategoryLocalizationValidator _validator;

    public BaseReportFundsExpendituresCategoryLocalizationValidatorTests()
    {
        _validator = new BaseReportFundsExpendituresCategoryLocalizationValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenName_IsNullOrEmpty(string? name)
    {
        var model = new UpdateReportFundsExpendituresCategoryLocalizationDto { Name = name! };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenName_IsTooShort()
    {
        var model = new UpdateReportFundsExpendituresCategoryLocalizationDto
        {
            Name = new string('a', ReportFundsExpendituresCategoryLocalizationConstants.NameMinLength - 1)
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenName_IsTooLong()
    {
        var model = new UpdateReportFundsExpendituresCategoryLocalizationDto
        {
            Name = new string('a', ReportFundsExpendituresCategoryLocalizationConstants.NameMaxLength + 1)
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenModel_IsValid()
    {
        var model = new UpdateReportFundsExpendituresCategoryLocalizationDto
        {
            Name = "Valid Name"
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }
}
