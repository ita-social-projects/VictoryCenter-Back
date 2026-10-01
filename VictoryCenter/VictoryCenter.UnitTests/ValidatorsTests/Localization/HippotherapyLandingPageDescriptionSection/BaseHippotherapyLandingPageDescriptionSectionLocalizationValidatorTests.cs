using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageDescriptionSection;
using VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageDescriptionSection;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.HippotherapyLandingPageDescriptionSection;

public class BaseHippotherapyLandingPageDescriptionSectionLocalizationValidatorTests
{
    private readonly BaseHippotherapyLandingPageDescriptionSectionLocalizationValidator _validator;

    public BaseHippotherapyLandingPageDescriptionSectionLocalizationValidatorTests()
    {
        _validator = new BaseHippotherapyLandingPageDescriptionSectionLocalizationValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenTitle_IsNullOrEmpty(string? title)
    {
        var model = new UpdateHippotherapyLandingPageDescriptionSectionLocalizationDto
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
        var model = new UpdateHippotherapyLandingPageDescriptionSectionLocalizationDto
        {
            Title = new string('a', HippotherapyLandingPageDescriptionSectionLocalizationConstants.TitleMaxLength + 1),
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
        var model = new UpdateHippotherapyLandingPageDescriptionSectionLocalizationDto
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
        var model = new UpdateHippotherapyLandingPageDescriptionSectionLocalizationDto
        {
            Title = "Valid title",
            Description = new string('a', HippotherapyLandingPageDescriptionSectionLocalizationConstants.DescriptionMaxLength + 1),
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Theory]
    [InlineData("<p>Hi</p>")]
    [InlineData("<p><br></p>")]
    public void Validate_ShouldHaveError_WhenTitle_VisibleTextIsTooShort(string title)
    {
        var model = new UpdateHippotherapyLandingPageDescriptionSectionLocalizationDto
        {
            Title = title,
            Description = "Valid description",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenTitle_WithHtmlMarkupHasVisibleTextWithinMaxLength()
    {
        var visibleText = new string('a', HippotherapyLandingPageDescriptionSectionLocalizationConstants.TitleMaxLength);
        var model = new UpdateHippotherapyLandingPageDescriptionSectionLocalizationDto
        {
            Title = $"<p><strong>{visibleText}</strong></p>",
            Description = "Valid description",
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
    }

    [Theory]
    [InlineData("<p><br></p>")]
    [InlineData("<p>Too short</p>")]
    public void Validate_ShouldHaveError_WhenDescription_VisibleTextIsTooShort(string description)
    {
        var model = new UpdateHippotherapyLandingPageDescriptionSectionLocalizationDto
        {
            Title = "Valid title",
            Description = description,
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenDescription_WithHtmlMarkupHasVisibleTextWithinMaxLength()
    {
        var visibleText = new string('a', HippotherapyLandingPageDescriptionSectionLocalizationConstants.DescriptionMaxLength);
        var model = new UpdateHippotherapyLandingPageDescriptionSectionLocalizationDto
        {
            Title = "Valid title",
            Description = $"<p><strong>{visibleText}</strong></p>",
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescription_VisibleTextExceedsMaxLength()
    {
        var visibleText = new string('a', HippotherapyLandingPageDescriptionSectionLocalizationConstants.DescriptionMaxLength + 1);
        var model = new UpdateHippotherapyLandingPageDescriptionSectionLocalizationDto
        {
            Title = "Valid title",
            Description = $"<p>{visibleText}</p>",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenModel_IsValid()
    {
        var model = new UpdateHippotherapyLandingPageDescriptionSectionLocalizationDto
        {
            Title = "Hippotherapy",
            Description = "Therapeutic horseback riding for children and veterans.",
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
