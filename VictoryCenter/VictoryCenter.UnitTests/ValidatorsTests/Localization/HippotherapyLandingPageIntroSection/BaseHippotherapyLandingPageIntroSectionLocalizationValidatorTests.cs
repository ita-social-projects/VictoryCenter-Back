using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
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
    public void Validate_ShouldHaveError_WhenTitle_VisibleTextIsTooShort()
    {
        var visibleText = new string('a', HippotherapyLandingPageIntroSectionLocalizationConstants.TitleMinLength - 1);
        var model = new UpdateHippotherapyLandingPageIntroSectionLocalizationDto
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
        var model = new UpdateHippotherapyLandingPageIntroSectionLocalizationDto
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
        var model = new UpdateHippotherapyLandingPageIntroSectionLocalizationDto
        {
            Title = new string('a', HippotherapyLandingPageIntroSectionLocalizationConstants.TitleMinLength - 1),
            Description = "Valid description",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenTitle_WithHtmlMarkupHasVisibleTextWithinMaxLength()
    {
        var visibleText = new string('a', HippotherapyLandingPageIntroSectionLocalizationConstants.TitleMaxLength);
        var model = new UpdateHippotherapyLandingPageIntroSectionLocalizationDto
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
        var visibleText = new string('a', HippotherapyLandingPageIntroSectionLocalizationConstants.DescriptionMinLength - 1);
        var model = new UpdateHippotherapyLandingPageIntroSectionLocalizationDto
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
        var model = new UpdateHippotherapyLandingPageIntroSectionLocalizationDto
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
        var model = new UpdateHippotherapyLandingPageIntroSectionLocalizationDto
        {
            Title = "Valid title",
            Description = new string('a', HippotherapyLandingPageIntroSectionLocalizationConstants.DescriptionMinLength - 1),
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenDescription_WithHtmlMarkupHasVisibleTextWithinMaxLength()
    {
        var visibleText = new string('a', HippotherapyLandingPageIntroSectionLocalizationConstants.DescriptionMaxLength);
        var model = new UpdateHippotherapyLandingPageIntroSectionLocalizationDto
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
        var visibleText = new string('a', HippotherapyLandingPageIntroSectionLocalizationConstants.DescriptionMaxLength + 1);
        var model = new UpdateHippotherapyLandingPageIntroSectionLocalizationDto
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
        var model = new UpdateHippotherapyLandingPageIntroSectionLocalizationDto
        {
            Title = string.Empty,
            Description = "Valid description",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateHippotherapyLandingPageIntroSectionLocalizationDto.Title)))
            .Only();
    }

    [Fact]
    public void Validate_ShouldHaveOnlyRequiredError_WhenDescription_IsEmpty()
    {
        var model = new UpdateHippotherapyLandingPageIntroSectionLocalizationDto
        {
            Title = "Valid title",
            Description = string.Empty,
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateHippotherapyLandingPageIntroSectionLocalizationDto.Description)))
            .Only();
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
