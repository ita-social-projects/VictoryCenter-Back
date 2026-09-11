using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.FeedbackReviews.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.FeedbackReviews;

namespace VictoryCenter.BLL.Validators.Localization.FeedbackReviews;

public class UpdateFeedbackReviewLocalizationValidator
    : AbstractValidator<UpdateFeedbackReviewLocalizationCommand>
{
    public UpdateFeedbackReviewLocalizationValidator()
    {
        RuleFor(command => command.EntityId).GreaterThan(0);
        RuleFor(command => command.LanguageId).GreaterThan(0);

        RuleFor(command => command.Localization.AuthorName)
            .Must(authorName => !string.IsNullOrWhiteSpace(authorName))
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(UpdateFeedbackReviewLocalizationDto.AuthorName)))
            .Must(authorName => string.IsNullOrWhiteSpace(authorName)
                || authorName.Trim().Length >= FeedbackReviewConstants.AuthorNameMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdateFeedbackReviewLocalizationDto.AuthorName),
                FeedbackReviewConstants.AuthorNameMinLength))
            .Must(authorName => string.IsNullOrWhiteSpace(authorName)
                || authorName.Trim().Length <= FeedbackReviewConstants.AuthorNameMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdateFeedbackReviewLocalizationDto.AuthorName),
                FeedbackReviewConstants.AuthorNameMaxLength));

        RuleFor(command => command.Localization.Text)
            .Must(text => !string.IsNullOrWhiteSpace(text))
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(UpdateFeedbackReviewLocalizationDto.Text)))
            .Must(text => string.IsNullOrWhiteSpace(text)
                || text.Trim().Length >= FeedbackReviewConstants.TextMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdateFeedbackReviewLocalizationDto.Text),
                FeedbackReviewConstants.TextMinLength))
            .Must(text => string.IsNullOrWhiteSpace(text)
                || text.Trim().Length <= FeedbackReviewConstants.TextMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdateFeedbackReviewLocalizationDto.Text),
                FeedbackReviewConstants.TextMaxLength));
    }
}
