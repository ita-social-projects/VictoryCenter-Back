using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.Localization.FeedbackReviews.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.FeedbackReviews;
using VictoryCenter.BLL.Validators.Localization.FeedbackReviews;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.FeedbackReviews;

public class CreateFeedbackReviewLocalizationValidatorTests
{
    private readonly CreateFeedbackReviewLocalizationValidator _validator = new();

    private static CreateFeedbackReviewLocalizationDto ValidDto => new()
    {
        EntityId = 1,
        LanguageId = 2,
        AuthorName = "John Doe",
        Text = "An English translation of the participant review."
    };

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenEntityIdIsNotPositive(long invalidEntityId)
    {
        var command = new CreateFeedbackReviewLocalizationCommand(ValidDto with { EntityId = invalidEntityId });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Localization.EntityId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenLanguageIdIsNotPositive(long invalidLanguageId)
    {
        var command = new CreateFeedbackReviewLocalizationCommand(ValidDto with { LanguageId = invalidLanguageId });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Localization.LanguageId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ShouldHaveError_WhenAuthorNameIsEmpty(string invalidAuthorName)
    {
        var command = new CreateFeedbackReviewLocalizationCommand(ValidDto with { AuthorName = invalidAuthorName });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Localization.AuthorName)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(CreateFeedbackReviewLocalizationDto.AuthorName)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenAuthorNameTooShort()
    {
        var command = new CreateFeedbackReviewLocalizationCommand(ValidDto with { AuthorName = "Jo" });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Localization.AuthorName)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateFeedbackReviewLocalizationDto.AuthorName),
                FeedbackReviewConstants.AuthorNameMinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenAuthorNameTooLong()
    {
        var command = new CreateFeedbackReviewLocalizationCommand(ValidDto with
        {
            AuthorName = new string('A', FeedbackReviewConstants.AuthorNameMaxLength + 1)
        });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Localization.AuthorName)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateFeedbackReviewLocalizationDto.AuthorName),
                FeedbackReviewConstants.AuthorNameMaxLength));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ShouldHaveError_WhenTextIsEmpty(string invalidText)
    {
        var command = new CreateFeedbackReviewLocalizationCommand(ValidDto with { Text = invalidText });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Localization.Text)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(CreateFeedbackReviewLocalizationDto.Text)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTextTooShort()
    {
        var command = new CreateFeedbackReviewLocalizationCommand(ValidDto with { Text = "Too short" });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Localization.Text)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateFeedbackReviewLocalizationDto.Text),
                FeedbackReviewConstants.TextMinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTextTooLong()
    {
        var command = new CreateFeedbackReviewLocalizationCommand(ValidDto with
        {
            Text = new string('A', FeedbackReviewConstants.TextMaxLength + 1)
        });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Localization.Text)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateFeedbackReviewLocalizationDto.Text),
                FeedbackReviewConstants.TextMaxLength));
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenDtoIsValid()
    {
        var command = new CreateFeedbackReviewLocalizationCommand(ValidDto);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
