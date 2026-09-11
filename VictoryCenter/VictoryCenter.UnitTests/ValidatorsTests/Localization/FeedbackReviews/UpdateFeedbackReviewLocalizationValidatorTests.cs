using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.Localization.FeedbackReviews.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.FeedbackReviews;
using VictoryCenter.BLL.Validators.Localization.FeedbackReviews;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.FeedbackReviews;

public class UpdateFeedbackReviewLocalizationValidatorTests
{
    private readonly UpdateFeedbackReviewLocalizationValidator _validator = new();

    private static UpdateFeedbackReviewLocalizationDto ValidDto => new()
    {
        AuthorName = "John Doe",
        Text = "An English translation of the participant review."
    };

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenEntityIdIsNotPositive(long invalidEntityId)
    {
        var command = new UpdateFeedbackReviewLocalizationCommand(invalidEntityId, 2, ValidDto);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.EntityId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenLanguageIdIsNotPositive(long invalidLanguageId)
    {
        var command = new UpdateFeedbackReviewLocalizationCommand(1, invalidLanguageId, ValidDto);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.LanguageId);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenAuthorNameIsEmpty()
    {
        var command = new UpdateFeedbackReviewLocalizationCommand(1, 2, ValidDto with { AuthorName = string.Empty });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Localization.AuthorName)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(UpdateFeedbackReviewLocalizationDto.AuthorName)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenAuthorNameTooShort()
    {
        var command = new UpdateFeedbackReviewLocalizationCommand(1, 2, ValidDto with { AuthorName = "Jo" });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Localization.AuthorName)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdateFeedbackReviewLocalizationDto.AuthorName),
                FeedbackReviewConstants.AuthorNameMinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTextIsEmpty()
    {
        var command = new UpdateFeedbackReviewLocalizationCommand(1, 2, ValidDto with { Text = string.Empty });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Localization.Text)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(UpdateFeedbackReviewLocalizationDto.Text)));
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenDtoIsValid()
    {
        var command = new UpdateFeedbackReviewLocalizationCommand(1, 2, ValidDto);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
