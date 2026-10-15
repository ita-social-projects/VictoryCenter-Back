using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageParticipantsSection;
using VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageParticipantsSection;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.HippotherapyLandingPageParticipantsSection;

public class HippotherapyLandingPageParticipantCardLocalizationItemValidatorTests
{
    private readonly HippotherapyLandingPageParticipantCardLocalizationItemValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenCardId_IsNotPositive(long cardId)
    {
        var model = new UpdateHippotherapyLandingPageParticipantCardLocalizationItemDto
        {
            CardId = cardId,
            Description = "Valid card description",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.CardId)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(UpdateHippotherapyLandingPageParticipantCardLocalizationItemDto.CardId)));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenDescription_IsNullOrEmpty(string? description)
    {
        var model = new UpdateHippotherapyLandingPageParticipantCardLocalizationItemDto
        {
            CardId = 1,
            Description = description!,
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescription_VisibleTextIsTooShort()
    {
        var visibleText = new string('a', HippotherapyLandingPageParticipantsSectionLocalizationConstants.CardDescriptionMinLength - 1);
        var model = new UpdateHippotherapyLandingPageParticipantCardLocalizationItemDto
        {
            CardId = 1,
            Description = $"<p>{visibleText}</p>",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescription_VisibleTextExceedsMaxLength()
    {
        var visibleText = new string('a', HippotherapyLandingPageParticipantsSectionLocalizationConstants.CardDescriptionMaxLength + 1);
        var model = new UpdateHippotherapyLandingPageParticipantCardLocalizationItemDto
        {
            CardId = 1,
            Description = $"<p>{visibleText}</p>",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenDescription_WithHtmlMarkupHasVisibleTextWithinMaxLength()
    {
        var visibleText = new string('a', HippotherapyLandingPageParticipantsSectionLocalizationConstants.CardDescriptionMaxLength);
        var model = new UpdateHippotherapyLandingPageParticipantCardLocalizationItemDto
        {
            CardId = 1,
            Description = $"<p><strong>{visibleText}</strong></p>",
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenModel_IsValid()
    {
        var model = new UpdateHippotherapyLandingPageParticipantCardLocalizationItemDto
        {
            CardId = 1,
            Description = "Therapeutic horseback riding for children and veterans.",
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
