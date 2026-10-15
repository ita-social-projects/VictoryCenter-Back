using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageScientificReferencesSection;
using VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageScientificReferencesSection;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.HippotherapyLandingPageScientificReferencesSection;

public class BaseHippotherapyLandingPageScientificReferencesSectionLocalizationValidatorTests
{
    private readonly BaseHippotherapyLandingPageScientificReferencesSectionLocalizationValidator _validator;

    public BaseHippotherapyLandingPageScientificReferencesSectionLocalizationValidatorTests()
    {
        _validator = new BaseHippotherapyLandingPageScientificReferencesSectionLocalizationValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenTitle_IsNullOrEmpty(string? title)
    {
        var model = new UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationDto
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
        var model = new UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationDto
        {
            Title = new string('a', HippotherapyLandingPageScientificReferencesSectionLocalizationConstants.TitleMaxLength + 1),
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
        var model = new UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationDto
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
        var model = new UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationDto
        {
            Title = "Valid title",
            Description = new string('a', HippotherapyLandingPageScientificReferencesSectionLocalizationConstants.DescriptionMaxLength + 1),
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitle_VisibleTextIsTooShort()
    {
        var visibleText = new string('a', HippotherapyLandingPageScientificReferencesSectionLocalizationConstants.TitleMinLength - 1);
        var model = new UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationDto
        {
            Title = $"<p>{visibleText}</p>",
            Description = "Valid description",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitle_HasNoVisibleText()
    {
        var model = new UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationDto
        {
            Title = "<p><br></p>",
            Description = "Valid description",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitle_PlainTextIsTooShort()
    {
        var model = new UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationDto
        {
            Title = new string('a', HippotherapyLandingPageScientificReferencesSectionLocalizationConstants.TitleMinLength - 1),
            Description = "Valid description",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenTitle_WithHtmlMarkupHasVisibleTextWithinMaxLength()
    {
        var visibleText = new string('a', HippotherapyLandingPageScientificReferencesSectionLocalizationConstants.TitleMaxLength);
        var model = new UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationDto
        {
            Title = $"<p><strong>{visibleText}</strong></p>",
            Description = "Valid description",
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescription_VisibleTextIsTooShort()
    {
        var visibleText = new string('a', HippotherapyLandingPageScientificReferencesSectionLocalizationConstants.DescriptionMinLength - 1);
        var model = new UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationDto
        {
            Title = "Valid title",
            Description = $"<p>{visibleText}</p>",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescription_HasNoVisibleText()
    {
        var model = new UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationDto
        {
            Title = "Valid title",
            Description = "<p><br></p>",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescription_PlainTextIsTooShort()
    {
        var model = new UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationDto
        {
            Title = "Valid title",
            Description = new string('a', HippotherapyLandingPageScientificReferencesSectionLocalizationConstants.DescriptionMinLength - 1),
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenDescription_WithHtmlMarkupHasVisibleTextWithinMaxLength()
    {
        var visibleText = new string('a', HippotherapyLandingPageScientificReferencesSectionLocalizationConstants.DescriptionMaxLength);
        var model = new UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationDto
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
        var visibleText = new string('a', HippotherapyLandingPageScientificReferencesSectionLocalizationConstants.DescriptionMaxLength + 1);
        var model = new UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationDto
        {
            Title = "Valid title",
            Description = $"<p>{visibleText}</p>",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_ShouldHaveOnlyRequiredError_WhenTitle_IsEmpty()
    {
        var model = new UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationDto
        {
            Title = string.Empty,
            Description = "Valid description",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationDto.Title)))
            .Only();
    }

    [Fact]
    public void Validate_ShouldHaveOnlyRequiredError_WhenDescription_IsEmpty()
    {
        var model = new UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationDto
        {
            Title = "Valid title",
            Description = string.Empty,
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationDto.Description)))
            .Only();
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenModel_IsValid()
    {
        var model = new UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationDto
        {
            Title = "Scientific references",
            Description = "Therapeutic horseback riding for children and veterans.",
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
