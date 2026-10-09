using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAdvantagesSection;
using VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageAdvantagesSection;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.HippotherapyLandingPageAdvantagesSection;

public class HippotherapyLandingPageAdvantageCardLocalizationItemValidatorTests
{
    private readonly HippotherapyLandingPageAdvantageCardLocalizationItemValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenCardId_IsNotPositive(long cardId)
    {
        var model = new UpdateHippotherapyLandingPageAdvantageCardLocalizationItemDto
        {
            CardId = cardId,
            Description = "Valid card description",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.CardId)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(UpdateHippotherapyLandingPageAdvantageCardLocalizationItemDto.CardId)));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenDescription_IsNullOrEmpty(string? description)
    {
        var model = new UpdateHippotherapyLandingPageAdvantageCardLocalizationItemDto
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
        var visibleText = new string('a', HippotherapyLandingPageAdvantagesSectionLocalizationConstants.CardDescriptionMinLength - 1);
        var model = new UpdateHippotherapyLandingPageAdvantageCardLocalizationItemDto
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
        var visibleText = new string('a', HippotherapyLandingPageAdvantagesSectionLocalizationConstants.CardDescriptionMaxLength + 1);
        var model = new UpdateHippotherapyLandingPageAdvantageCardLocalizationItemDto
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
        var visibleText = new string('a', HippotherapyLandingPageAdvantagesSectionLocalizationConstants.CardDescriptionMaxLength);
        var model = new UpdateHippotherapyLandingPageAdvantageCardLocalizationItemDto
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
        var model = new UpdateHippotherapyLandingPageAdvantageCardLocalizationItemDto
        {
            CardId = 1,
            Description = "Therapeutic horseback riding for children and veterans.",
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
