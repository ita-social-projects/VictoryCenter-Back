using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAnalysisSection;
using VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageAnalysisSection;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.HippotherapyLandingPageAnalysisSection;

public class BaseHippotherapyLandingPageAnalysisSectionLocalizationValidatorTests
{
    private readonly BaseHippotherapyLandingPageAnalysisSectionLocalizationValidator _validator;

    public BaseHippotherapyLandingPageAnalysisSectionLocalizationValidatorTests()
    {
        _validator = new BaseHippotherapyLandingPageAnalysisSectionLocalizationValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenTitle_IsNullOrEmpty(string? title)
    {
        var model = new UpdateHippotherapyLandingPageAnalysisSectionLocalizationDto
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
        var model = new UpdateHippotherapyLandingPageAnalysisSectionLocalizationDto
        {
            Title = new string('a', HippotherapyLandingPageAnalysisSectionLocalizationConstants.TitleMaxLength + 1),
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
        var model = new UpdateHippotherapyLandingPageAnalysisSectionLocalizationDto
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
        var model = new UpdateHippotherapyLandingPageAnalysisSectionLocalizationDto
        {
            Title = "Valid title",
            Description = new string('a', HippotherapyLandingPageAnalysisSectionLocalizationConstants.DescriptionMaxLength + 1),
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Theory]
    [InlineData("<p>Hi</p>")]
    [InlineData("<p><br></p>")]
    public void Validate_ShouldHaveError_WhenTitle_VisibleTextIsTooShort(string title)
    {
        var model = new UpdateHippotherapyLandingPageAnalysisSectionLocalizationDto
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
        var visibleText = new string('a', HippotherapyLandingPageAnalysisSectionLocalizationConstants.TitleMaxLength);
        var model = new UpdateHippotherapyLandingPageAnalysisSectionLocalizationDto
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
        var model = new UpdateHippotherapyLandingPageAnalysisSectionLocalizationDto
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
        var visibleText = new string('a', HippotherapyLandingPageAnalysisSectionLocalizationConstants.DescriptionMaxLength);
        var model = new UpdateHippotherapyLandingPageAnalysisSectionLocalizationDto
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
        var visibleText = new string('a', HippotherapyLandingPageAnalysisSectionLocalizationConstants.DescriptionMaxLength + 1);
        var model = new UpdateHippotherapyLandingPageAnalysisSectionLocalizationDto
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
        var model = new UpdateHippotherapyLandingPageAnalysisSectionLocalizationDto
        {
            Title = "Analysis",
            Description = "Therapeutic horseback riding for children and veterans.",
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
