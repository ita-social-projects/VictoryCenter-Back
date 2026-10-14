using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventsIntroSection;
using VictoryCenter.BLL.Validators.Localization.EventsIntroSection;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.EventsIntroSection;

public class BaseEventsIntroSectionLocalizationValidatorTests
{
    private readonly BaseEventsIntroSectionLocalizationValidator _validator;

    public BaseEventsIntroSectionLocalizationValidatorTests()
    {
        _validator = new BaseEventsIntroSectionLocalizationValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenEventsBlockTitle_IsNullOrEmpty(string? title)
    {
        var model = new UpdateEventsIntroSectionLocalizationDto
        {
            EventsBlockTitle = title!,
            PageDescription = "Valid description text here.",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.EventsBlockTitle);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenEventsBlockTitle_IsTooLong()
    {
        var model = new UpdateEventsIntroSectionLocalizationDto
        {
            EventsBlockTitle = new string('a', EventsIntroSectionLocalizationConstants.EventsBlockTitleMaxLength + 1),
            PageDescription = "Valid description text here.",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.EventsBlockTitle);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenPageDescription_IsNullOrEmpty(string? description)
    {
        var model = new UpdateEventsIntroSectionLocalizationDto
        {
            EventsBlockTitle = "Valid title",
            PageDescription = description!,
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.PageDescription);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPageDescription_IsTooLong()
    {
        var model = new UpdateEventsIntroSectionLocalizationDto
        {
            EventsBlockTitle = "Valid title",
            PageDescription = new string('a', EventsIntroSectionLocalizationConstants.PageDescriptionMaxLength + 1),
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.PageDescription);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenEventsBlockTitle_VisibleTextIsTooShort()
    {
        var visibleText = new string('a', EventsIntroSectionLocalizationConstants.EventsBlockTitleMinLength - 1);
        var model = new UpdateEventsIntroSectionLocalizationDto
        {
            EventsBlockTitle = $"<p>{visibleText}</p>",
            PageDescription = "Valid description text here.",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.EventsBlockTitle);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenEventsBlockTitle_HasNoVisibleText()
    {
        var model = new UpdateEventsIntroSectionLocalizationDto
        {
            EventsBlockTitle = "<p><br></p>",
            PageDescription = "Valid description text here.",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.EventsBlockTitle);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenEventsBlockTitle_PlainTextIsTooShort()
    {
        var model = new UpdateEventsIntroSectionLocalizationDto
        {
            EventsBlockTitle = new string('a', EventsIntroSectionLocalizationConstants.EventsBlockTitleMinLength - 1),
            PageDescription = "Valid description text here.",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.EventsBlockTitle);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenEventsBlockTitle_WithHtmlMarkupHasVisibleTextWithinMaxLength()
    {
        var visibleText = new string('a', EventsIntroSectionLocalizationConstants.EventsBlockTitleMaxLength);
        var model = new UpdateEventsIntroSectionLocalizationDto
        {
            EventsBlockTitle = $"<p><strong>{visibleText}</strong></p>",
            PageDescription = "Valid description text here.",
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.EventsBlockTitle);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPageDescription_VisibleTextIsTooShort()
    {
        var visibleText = new string('a', EventsIntroSectionLocalizationConstants.PageDescriptionMinLength - 1);
        var model = new UpdateEventsIntroSectionLocalizationDto
        {
            EventsBlockTitle = "Valid title",
            PageDescription = $"<p>{visibleText}</p>",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.PageDescription);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPageDescription_HasNoVisibleText()
    {
        var model = new UpdateEventsIntroSectionLocalizationDto
        {
            EventsBlockTitle = "Valid title",
            PageDescription = "<p><br></p>",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.PageDescription);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPageDescription_PlainTextIsTooShort()
    {
        var model = new UpdateEventsIntroSectionLocalizationDto
        {
            EventsBlockTitle = "Valid title",
            PageDescription = new string('a', EventsIntroSectionLocalizationConstants.PageDescriptionMinLength - 1),
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.PageDescription);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenPageDescription_WithHtmlMarkupHasVisibleTextWithinMaxLength()
    {
        var visibleText = new string('a', EventsIntroSectionLocalizationConstants.PageDescriptionMaxLength);
        var model = new UpdateEventsIntroSectionLocalizationDto
        {
            EventsBlockTitle = "Valid title",
            PageDescription = $"<p><strong>{visibleText}</strong></p>",
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.PageDescription);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPageDescription_VisibleTextExceedsMaxLength()
    {
        var visibleText = new string('a', EventsIntroSectionLocalizationConstants.PageDescriptionMaxLength + 1);
        var model = new UpdateEventsIntroSectionLocalizationDto
        {
            EventsBlockTitle = "Valid title",
            PageDescription = $"<p>{visibleText}</p>",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.PageDescription);
    }

    [Fact]
    public void Validate_ShouldHaveOnlyRequiredError_WhenEventsBlockTitle_IsEmpty()
    {
        var model = new UpdateEventsIntroSectionLocalizationDto
        {
            EventsBlockTitle = string.Empty,
            PageDescription = "Valid description text here.",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.EventsBlockTitle)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateEventsIntroSectionLocalizationDto.EventsBlockTitle)))
            .Only();
    }

    [Fact]
    public void Validate_ShouldHaveOnlyRequiredError_WhenPageDescription_IsEmpty()
    {
        var model = new UpdateEventsIntroSectionLocalizationDto
        {
            EventsBlockTitle = "Valid title",
            PageDescription = string.Empty,
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.PageDescription)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateEventsIntroSectionLocalizationDto.PageDescription)))
            .Only();
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenModel_IsValid()
    {
        var model = new UpdateEventsIntroSectionLocalizationDto
        {
            EventsBlockTitle = "What has been happening",
            PageDescription = "This section is about the moments when what we believe in becomes real.",
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
