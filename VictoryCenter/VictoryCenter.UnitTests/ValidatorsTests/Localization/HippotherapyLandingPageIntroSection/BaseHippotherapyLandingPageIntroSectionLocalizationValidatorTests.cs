using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageIntroSection;
using VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageIntroSection;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.HippotherapyLandingPageIntroSection;

public class BaseHippotherapyLandingPageIntroSectionLocalizationValidatorTests
{
    private readonly BaseHippotherapyLandingPageIntroSectionLocalizationValidator _validator;

    public BaseHippotherapyLandingPageIntroSectionLocalizationValidatorTests()
    {
        _validator = new BaseHippotherapyLandingPageIntroSectionLocalizationValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenTitle_IsNullOrEmpty(string? title)
    {
        var model = new UpdateHippotherapyLandingPageIntroSectionLocalizationDto
        {
            Title = title!,
            Description = "Valid description",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitle_IsTooLong()
    {
        var model = new UpdateHippotherapyLandingPageIntroSectionLocalizationDto
        {
            Title = new string('a', HippotherapyLandingPageIntroSectionLocalizationConstants.TitleMaxLength + 1),
            Description = "Valid description",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenDescription_IsNullOrEmpty(string? description)
    {
        var model = new UpdateHippotherapyLandingPageIntroSectionLocalizationDto
        {
            Title = "Valid title",
            Description = description!,
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescription_IsTooLong()
    {
        var model = new UpdateHippotherapyLandingPageIntroSectionLocalizationDto
        {
            Title = "Valid title",
            Description = new string('a', HippotherapyLandingPageIntroSectionLocalizationConstants.DescriptionMaxLength + 1),
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenModel_IsValid()
    {
        var model = new UpdateHippotherapyLandingPageIntroSectionLocalizationDto
        {
            Title = "Hippotherapy",
            Description = "Therapeutic horseback riding for children and veterans.",
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
