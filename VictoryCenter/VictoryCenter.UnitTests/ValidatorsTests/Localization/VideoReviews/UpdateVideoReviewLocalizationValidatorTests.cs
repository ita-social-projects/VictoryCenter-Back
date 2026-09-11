using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.Localization.VideoReviews.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.VideoReviews;
using VictoryCenter.BLL.Validators.Localization.VideoReviews;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.VideoReviews;

public class UpdateVideoReviewLocalizationValidatorTests
{
    private readonly UpdateVideoReviewLocalizationValidator _validator = new();

    private static UpdateVideoReviewLocalizationDto ValidDto => new()
    {
        Title = "English video review title"
    };

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenEntityIdIsNotPositive(long invalidEntityId)
    {
        var command = new UpdateVideoReviewLocalizationCommand(invalidEntityId, 2, ValidDto);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.EntityId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenLanguageIdIsNotPositive(long invalidLanguageId)
    {
        var command = new UpdateVideoReviewLocalizationCommand(1, invalidLanguageId, ValidDto);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.LanguageId);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitleIsEmpty()
    {
        var command = new UpdateVideoReviewLocalizationCommand(1, 2, ValidDto with { Title = string.Empty });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Localization.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(UpdateVideoReviewLocalizationDto.Title)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitleTooShort()
    {
        var command = new UpdateVideoReviewLocalizationCommand(1, 2, ValidDto with { Title = "Hi" });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Localization.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdateVideoReviewLocalizationDto.Title),
                VideoReviewConstants.TitleMinLength));
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenDtoIsValid()
    {
        var command = new UpdateVideoReviewLocalizationCommand(1, 2, ValidDto);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
