using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAdvantagesSection;
using VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageAdvantagesSection;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.HippotherapyLandingPageAdvantagesSection;

public class BaseHippotherapyLandingPageAdvantagesSectionLocalizationValidatorTests
{
    private readonly BaseHippotherapyLandingPageAdvantagesSectionLocalizationValidator _validator;

    public BaseHippotherapyLandingPageAdvantagesSectionLocalizationValidatorTests()
    {
        _validator = new BaseHippotherapyLandingPageAdvantagesSectionLocalizationValidator(
            new HippotherapyLandingPageAdvantageCardLocalizationItemValidator());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenTitle_IsNullOrEmpty(string? title)
    {
        var model = new UpdateHippotherapyLandingPageAdvantagesSectionLocalizationDto
        {
            Title = title!,
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitle_IsTooLong()
    {
        var model = new UpdateHippotherapyLandingPageAdvantagesSectionLocalizationDto
        {
            Title = new string('a', HippotherapyLandingPageAdvantagesSectionLocalizationConstants.TitleMaxLength + 1),
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitle_VisibleTextIsTooShort()
    {
        var visibleText = new string('a', HippotherapyLandingPageAdvantagesSectionLocalizationConstants.TitleMinLength - 1);
        var model = new UpdateHippotherapyLandingPageAdvantagesSectionLocalizationDto
        {
            Title = $"<p>{visibleText}</p>",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitle_HasNoVisibleText()
    {
        var model = new UpdateHippotherapyLandingPageAdvantagesSectionLocalizationDto
        {
            Title = "<p><br></p>",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitle_PlainTextIsTooShort()
    {
        var model = new UpdateHippotherapyLandingPageAdvantagesSectionLocalizationDto
        {
            Title = new string('a', HippotherapyLandingPageAdvantagesSectionLocalizationConstants.TitleMinLength - 1),
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenTitle_WithHtmlMarkupHasVisibleTextWithinMaxLength()
    {
        var visibleText = new string('a', HippotherapyLandingPageAdvantagesSectionLocalizationConstants.TitleMaxLength);
        var model = new UpdateHippotherapyLandingPageAdvantagesSectionLocalizationDto
        {
            Title = $"<p><strong>{visibleText}</strong></p>",
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitle_VisibleTextExceedsMaxLength()
    {
        var visibleText = new string('a', HippotherapyLandingPageAdvantagesSectionLocalizationConstants.TitleMaxLength + 1);
        var model = new UpdateHippotherapyLandingPageAdvantagesSectionLocalizationDto
        {
            Title = $"<p>{visibleText}</p>",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_ShouldHaveOnlyRequiredError_WhenTitle_IsEmpty()
    {
        var model = new UpdateHippotherapyLandingPageAdvantagesSectionLocalizationDto
        {
            Title = string.Empty,
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateHippotherapyLandingPageAdvantagesSectionLocalizationDto.Title)))
            .Only();
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenModel_IsValid()
    {
        var model = new UpdateHippotherapyLandingPageAdvantagesSectionLocalizationDto
        {
            Title = "Why this approach",
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenCards_AreValid()
    {
        var model = new UpdateHippotherapyLandingPageAdvantagesSectionLocalizationDto
        {
            Title = "Why this approach",
            Cards =
            [
                new UpdateHippotherapyLandingPageAdvantageCardLocalizationItemDto { CardId = 1, Description = "Valid card description 1" },
                new UpdateHippotherapyLandingPageAdvantageCardLocalizationItemDto { CardId = 2, Description = "Valid card description 2" },
            ],
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenCards_IsNull()
    {
        var model = new UpdateHippotherapyLandingPageAdvantagesSectionLocalizationDto
        {
            Title = "Why this approach",
            Cards = null!,
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Cards)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateHippotherapyLandingPageAdvantagesSectionLocalizationDto.Cards)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenCards_ContainDuplicateCardIds()
    {
        var model = new UpdateHippotherapyLandingPageAdvantagesSectionLocalizationDto
        {
            Title = "Why this approach",
            Cards =
            [
                new UpdateHippotherapyLandingPageAdvantageCardLocalizationItemDto { CardId = 1, Description = "Valid card description 1" },
                new UpdateHippotherapyLandingPageAdvantageCardLocalizationItemDto { CardId = 1, Description = "Valid card description 2" },
            ],
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Cards)
            .WithErrorMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(nameof(UpdateHippotherapyLandingPageAdvantagesSectionLocalizationDto.Cards)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenCardItem_IsInvalid()
    {
        var model = new UpdateHippotherapyLandingPageAdvantagesSectionLocalizationDto
        {
            Title = "Why this approach",
            Cards = [new UpdateHippotherapyLandingPageAdvantageCardLocalizationItemDto { CardId = 0, Description = "short" }],
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor("Cards[0].CardId");
        result.ShouldHaveValidationErrorFor("Cards[0].Description");
    }
}
