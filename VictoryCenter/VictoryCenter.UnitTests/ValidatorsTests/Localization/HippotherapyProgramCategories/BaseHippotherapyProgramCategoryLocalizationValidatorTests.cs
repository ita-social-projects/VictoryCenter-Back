using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramCategories;
using VictoryCenter.BLL.Validators.Localization.HippotherapyProgramCategories;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.HippotherapyProgramCategories;

public class BaseHippotherapyProgramCategoryLocalizationValidatorTests
{
    private readonly BaseHippotherapyProgramCategoryLocalizationValidator _validator;

    public BaseHippotherapyProgramCategoryLocalizationValidatorTests()
    {
        _validator = new BaseHippotherapyProgramCategoryLocalizationValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenName_IsNullOrEmpty(string? name)
    {
        var model = new UpdateHippotherapyProgramCategoryLocalizationDto { Name = name! };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenName_IsTooShort()
    {
        var model = new UpdateHippotherapyProgramCategoryLocalizationDto
        {
            Name = new string('a', HippotherapyProgramCategoryLocalizationConstants.NameMinLength - 1)
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenName_IsTooLong()
    {
        var model = new UpdateHippotherapyProgramCategoryLocalizationDto
        {
            Name = new string('a', HippotherapyProgramCategoryLocalizationConstants.NameMaxLength + 1)
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenModel_IsValid()
    {
        var model = new UpdateHippotherapyProgramCategoryLocalizationDto
        {
            Name = "Valid Name"
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }
}
