using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.Localization.VideoReviews.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.VideoReviews;
using VictoryCenter.BLL.Validators.Localization.VideoReviews;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.VideoReviews;

public class CreateVideoReviewLocalizationValidatorTests
{
    private readonly CreateVideoReviewLocalizationValidator _validator = new();

    private static CreateVideoReviewLocalizationDto ValidDto => new()
    {
        EntityId = 1,
        LanguageId = 2,
        Title = "English video review title"
    };

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenEntityIdIsNotPositive(long invalidEntityId)
    {
        var command = new CreateVideoReviewLocalizationCommand(ValidDto with { EntityId = invalidEntityId });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Localization.EntityId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenLanguageIdIsNotPositive(long invalidLanguageId)
    {
        var command = new CreateVideoReviewLocalizationCommand(ValidDto with { LanguageId = invalidLanguageId });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Localization.LanguageId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ShouldHaveError_WhenTitleIsEmpty(string invalidTitle)
    {
        var command = new CreateVideoReviewLocalizationCommand(ValidDto with { Title = invalidTitle });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Localization.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(CreateVideoReviewLocalizationDto.Title)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitleTooShort()
    {
        var command = new CreateVideoReviewLocalizationCommand(ValidDto with { Title = "Hi" });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Localization.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateVideoReviewLocalizationDto.Title),
                VideoReviewConstants.TitleMinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitleTooLong()
    {
        var command = new CreateVideoReviewLocalizationCommand(ValidDto with
        {
            Title = new string('A', VideoReviewConstants.TitleMaxLength + 1)
        });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Localization.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateVideoReviewLocalizationDto.Title),
                VideoReviewConstants.TitleMaxLength));
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenDtoIsValid()
    {
        var command = new CreateVideoReviewLocalizationCommand(ValidDto);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
