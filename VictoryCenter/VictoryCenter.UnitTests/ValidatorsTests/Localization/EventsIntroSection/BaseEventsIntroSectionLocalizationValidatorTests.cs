using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventsIntroSection;
using VictoryCenter.BLL.Validators.Localization.EventsIntroSection;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.EventsIntroSection;

public class BaseEventsIntroSectionLocalizationValidatorTests
{
    private readonly BaseEventsIntroSectionLocalizationValidator _validator = new();

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenOnlyEventsBlockTitleIsProvided()
    {
        var result = _validator.TestValidate(new UpdateEventsIntroSectionLocalizationDto
        {
            EventsBlockTitle = "Valid title",
        });

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenOnlyPageDescriptionIsProvided()
    {
        var result = _validator.TestValidate(new UpdateEventsIntroSectionLocalizationDto
        {
            PageDescription = "Valid description text.",
        });

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenNoFieldsAreProvided()
    {
        var result = _validator.TestValidate(new UpdateEventsIntroSectionLocalizationDto());

        result.ShouldHaveAnyValidationError();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("<p><br></p>")]
    public void Validate_ShouldHaveError_WhenProvidedContentHasNoVisibleText(string content)
    {
        var result = _validator.TestValidate(new UpdateEventsIntroSectionLocalizationDto
        {
            EventsBlockTitle = content,
        });

        result.ShouldHaveValidationErrorFor(model => model.EventsBlockTitle);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenEventsBlockTitleIsOutsideVisibleLengthLimits()
    {
        var tooShort = new string('a', EventsIntroSectionLocalizationConstants.EventsBlockTitleMinLength - 1);
        var tooLong = new string('a', EventsIntroSectionLocalizationConstants.EventsBlockTitleMaxLength + 1);

        _validator.TestValidate(new UpdateEventsIntroSectionLocalizationDto { EventsBlockTitle = tooShort })
            .ShouldHaveValidationErrorFor(model => model.EventsBlockTitle);
        _validator.TestValidate(new UpdateEventsIntroSectionLocalizationDto { EventsBlockTitle = tooLong })
            .ShouldHaveValidationErrorFor(model => model.EventsBlockTitle);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPageDescriptionIsOutsideVisibleLengthLimits()
    {
        var tooShort = new string('a', EventsIntroSectionLocalizationConstants.PageDescriptionMinLength - 1);
        var tooLong = new string('a', EventsIntroSectionLocalizationConstants.PageDescriptionMaxLength + 1);

        _validator.TestValidate(new UpdateEventsIntroSectionLocalizationDto { PageDescription = tooShort })
            .ShouldHaveValidationErrorFor(model => model.PageDescription);
        _validator.TestValidate(new UpdateEventsIntroSectionLocalizationDto { PageDescription = tooLong })
            .ShouldHaveValidationErrorFor(model => model.PageDescription);
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenValuesAreAtVisibleLengthBoundaries()
    {
        var result = _validator.TestValidate(new UpdateEventsIntroSectionLocalizationDto
        {
            EventsBlockTitle = new string('a', EventsIntroSectionLocalizationConstants.EventsBlockTitleMinLength),
            PageDescription = new string('a', EventsIntroSectionLocalizationConstants.PageDescriptionMaxLength),
        });

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldUseVisibleHtmlLengthInsteadOfRawStringLength()
    {
        var visibleText = new string('a', EventsIntroSectionLocalizationConstants.EventsBlockTitleMaxLength);
        var result = _validator.TestValidate(new UpdateEventsIntroSectionLocalizationDto
        {
            EventsBlockTitle = $"<p><strong>{visibleText}</strong></p>",
        });

        result.ShouldNotHaveAnyValidationErrors();
    }
}
