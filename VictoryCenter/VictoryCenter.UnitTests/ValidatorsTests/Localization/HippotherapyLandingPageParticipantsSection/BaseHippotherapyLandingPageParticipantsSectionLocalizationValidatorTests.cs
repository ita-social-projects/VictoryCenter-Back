using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageParticipantsSection;
using VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageParticipantsSection;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.HippotherapyLandingPageParticipantsSection;

public class BaseHippotherapyLandingPageParticipantsSectionLocalizationValidatorTests
{
    private readonly BaseHippotherapyLandingPageParticipantsSectionLocalizationValidator _validator;

    public BaseHippotherapyLandingPageParticipantsSectionLocalizationValidatorTests()
    {
        _validator = new BaseHippotherapyLandingPageParticipantsSectionLocalizationValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenTitle_IsNullOrEmpty(string? title)
    {
        var model = new UpdateHippotherapyLandingPageParticipantsSectionLocalizationDto
        {
            Title = title!,
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitle_IsTooLong()
    {
        var model = new UpdateHippotherapyLandingPageParticipantsSectionLocalizationDto
        {
            Title = new string('a', HippotherapyLandingPageParticipantsSectionLocalizationConstants.TitleMaxLength + 1),
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitle_VisibleTextIsTooShort()
    {
        var visibleText = new string('a', HippotherapyLandingPageParticipantsSectionLocalizationConstants.TitleMinLength - 1);
        var model = new UpdateHippotherapyLandingPageParticipantsSectionLocalizationDto
        {
            Title = $"<p>{visibleText}</p>",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitle_HasNoVisibleText()
    {
        var model = new UpdateHippotherapyLandingPageParticipantsSectionLocalizationDto
        {
            Title = "<p><br></p>",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitle_PlainTextIsTooShort()
    {
        var model = new UpdateHippotherapyLandingPageParticipantsSectionLocalizationDto
        {
            Title = new string('a', HippotherapyLandingPageParticipantsSectionLocalizationConstants.TitleMinLength - 1),
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenTitle_WithHtmlMarkupHasVisibleTextWithinMaxLength()
    {
        var visibleText = new string('a', HippotherapyLandingPageParticipantsSectionLocalizationConstants.TitleMaxLength);
        var model = new UpdateHippotherapyLandingPageParticipantsSectionLocalizationDto
        {
            Title = $"<p><strong>{visibleText}</strong></p>",
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitle_VisibleTextExceedsMaxLength()
    {
        var visibleText = new string('a', HippotherapyLandingPageParticipantsSectionLocalizationConstants.TitleMaxLength + 1);
        var model = new UpdateHippotherapyLandingPageParticipantsSectionLocalizationDto
        {
            Title = $"<p>{visibleText}</p>",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_ShouldHaveOnlyRequiredError_WhenTitle_IsEmpty()
    {
        var model = new UpdateHippotherapyLandingPageParticipantsSectionLocalizationDto
        {
            Title = string.Empty,
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateHippotherapyLandingPageParticipantsSectionLocalizationDto.Title)))
            .Only();
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenModel_IsValid()
    {
        var model = new UpdateHippotherapyLandingPageParticipantsSectionLocalizationDto
        {
            Title = "Who the programs are for",
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
