using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageHippoventionCenterSection;
using VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageHippoventionCenterSection;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.HippotherapyLandingPageHippoventionCenterSection;

public class BaseHippotherapyLandingPageHippoventionCenterSectionLocalizationValidatorTests
{
    private readonly BaseHippotherapyLandingPageHippoventionCenterSectionLocalizationValidator _validator;

    public BaseHippotherapyLandingPageHippoventionCenterSectionLocalizationValidatorTests()
    {
        _validator = new BaseHippotherapyLandingPageHippoventionCenterSectionLocalizationValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenTitle_IsNullOrEmpty(string? title)
    {
        var model = new UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto
        {
            Title = title!,
            Description = "Valid description",
            Pros = "Valid pros text",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitle_IsTooLong()
    {
        var model = new UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto
        {
            Title = new string('a', HippotherapyLandingPageHippoventionCenterSectionLocalizationConstants.TitleMaxLength + 1),
            Description = "Valid description",
            Pros = "Valid pros text",
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
        var model = new UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto
        {
            Title = "Valid title",
            Description = description!,
            Pros = "Valid pros text",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenPros_IsNullOrEmpty(string? pros)
    {
        var model = new UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto
        {
            Title = "Valid title",
            Description = "Valid description",
            Pros = pros!,
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Pros);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescription_IsTooLong()
    {
        var model = new UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto
        {
            Title = "Valid title",
            Description = new string('a', HippotherapyLandingPageHippoventionCenterSectionLocalizationConstants.DescriptionMaxLength + 1),
            Pros = "Valid pros text",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPros_IsTooLong()
    {
        var model = new UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto
        {
            Title = "Valid title",
            Description = "Valid description",
            Pros = new string('a', HippotherapyLandingPageHippoventionCenterSectionLocalizationConstants.ProsMaxLength + 1),
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Pros);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitle_VisibleTextIsTooShort()
    {
        var visibleText = new string('a', HippotherapyLandingPageHippoventionCenterSectionLocalizationConstants.TitleMinLength - 1);
        var model = new UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto
        {
            Title = $"<p>{visibleText}</p>",
            Description = "Valid description",
            Pros = "Valid pros text",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitle_HasNoVisibleText()
    {
        var model = new UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto
        {
            Title = "<p><br></p>",
            Description = "Valid description",
            Pros = "Valid pros text",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitle_PlainTextIsTooShort()
    {
        var model = new UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto
        {
            Title = new string('a', HippotherapyLandingPageHippoventionCenterSectionLocalizationConstants.TitleMinLength - 1),
            Description = "Valid description",
            Pros = "Valid pros text",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenTitle_WithHtmlMarkupHasVisibleTextWithinMaxLength()
    {
        var visibleText = new string('a', HippotherapyLandingPageHippoventionCenterSectionLocalizationConstants.TitleMaxLength);
        var model = new UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto
        {
            Title = $"<p><strong>{visibleText}</strong></p>",
            Description = "Valid description",
            Pros = "Valid pros text",
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescription_VisibleTextIsTooShort()
    {
        var visibleText = new string('a', HippotherapyLandingPageHippoventionCenterSectionLocalizationConstants.DescriptionMinLength - 1);
        var model = new UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto
        {
            Title = "Valid title",
            Description = $"<p>{visibleText}</p>",
            Pros = "Valid pros text",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPros_VisibleTextIsTooShort()
    {
        var visibleText = new string('a', HippotherapyLandingPageHippoventionCenterSectionLocalizationConstants.ProsMinLength - 1);
        var model = new UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto
        {
            Title = "Valid title",
            Description = "Valid description",
            Pros = $"<p>{visibleText}</p>",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Pros);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescription_HasNoVisibleText()
    {
        var model = new UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto
        {
            Title = "Valid title",
            Description = "<p><br></p>",
            Pros = "Valid pros text",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPros_HasNoVisibleText()
    {
        var model = new UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto
        {
            Title = "Valid title",
            Description = "Valid description",
            Pros = "<p><br></p>",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Pros);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescription_PlainTextIsTooShort()
    {
        var model = new UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto
        {
            Title = "Valid title",
            Description = new string('a', HippotherapyLandingPageHippoventionCenterSectionLocalizationConstants.DescriptionMinLength - 1),
            Pros = "Valid pros text",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPros_PlainTextIsTooShort()
    {
        var model = new UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto
        {
            Title = "Valid title",
            Description = "Valid description",
            Pros = new string('a', HippotherapyLandingPageHippoventionCenterSectionLocalizationConstants.ProsMinLength - 1),
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Pros);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenDescription_WithHtmlMarkupHasVisibleTextWithinMaxLength()
    {
        var visibleText = new string('a', HippotherapyLandingPageHippoventionCenterSectionLocalizationConstants.DescriptionMaxLength);
        var model = new UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto
        {
            Title = "Valid title",
            Description = $"<p><strong>{visibleText}</strong></p>",
            Pros = "Valid pros text",
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenPros_WithHtmlMarkupHasVisibleTextWithinMaxLength()
    {
        var visibleText = new string('a', HippotherapyLandingPageHippoventionCenterSectionLocalizationConstants.ProsMaxLength);
        var model = new UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto
        {
            Title = "Valid title",
            Description = "Valid description",
            Pros = $"<p><strong>{visibleText}</strong></p>",
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Pros);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescription_VisibleTextExceedsMaxLength()
    {
        var visibleText = new string('a', HippotherapyLandingPageHippoventionCenterSectionLocalizationConstants.DescriptionMaxLength + 1);
        var model = new UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto
        {
            Title = "Valid title",
            Description = $"<p>{visibleText}</p>",
            Pros = "Valid pros text",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPros_VisibleTextExceedsMaxLength()
    {
        var visibleText = new string('a', HippotherapyLandingPageHippoventionCenterSectionLocalizationConstants.ProsMaxLength + 1);
        var model = new UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto
        {
            Title = "Valid title",
            Description = "Valid description",
            Pros = $"<p>{visibleText}</p>",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Pros);
    }

    [Fact]
    public void Validate_ShouldHaveOnlyRequiredError_WhenTitle_IsEmpty()
    {
        var model = new UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto
        {
            Title = string.Empty,
            Description = "Valid description",
            Pros = "Valid pros text",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto.Title)))
            .Only();
    }

    [Fact]
    public void Validate_ShouldHaveOnlyRequiredError_WhenDescription_IsEmpty()
    {
        var model = new UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto
        {
            Title = "Valid title",
            Description = string.Empty,
            Pros = "Valid pros text",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto.Description)))
            .Only();
    }

    [Fact]
    public void Validate_ShouldHaveOnlyRequiredError_WhenPros_IsEmpty()
    {
        var model = new UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto
        {
            Title = "Valid title",
            Description = "Valid description",
            Pros = string.Empty,
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Pros)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto.Pros)))
            .Only();
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenModel_IsValid()
    {
        var model = new UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto
        {
            Title = "HippoventionCenter",
            Description = "Therapeutic horseback riding for children and veterans.",
            Pros = "Valid pros text",
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
