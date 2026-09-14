using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.FeedbackReviews.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.FeedbackReviews;

namespace VictoryCenter.BLL.Validators.Localization.FeedbackReviews;

public class CreateFeedbackReviewLocalizationValidator
    : AbstractValidator<CreateFeedbackReviewLocalizationCommand>
{
    public CreateFeedbackReviewLocalizationValidator()
    {
        RuleFor(command => command.Localization.EntityId).GreaterThan(0);
        RuleFor(command => command.Localization.LanguageId).GreaterThan(0);

        RuleFor(command => command.Localization.AuthorName)
            .Must(authorName => !string.IsNullOrWhiteSpace(authorName))
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(CreateFeedbackReviewLocalizationDto.AuthorName)))
            .Must(authorName => string.IsNullOrWhiteSpace(authorName)
                || authorName.Trim().Length >= FeedbackReviewConstants.AuthorNameMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateFeedbackReviewLocalizationDto.AuthorName),
                FeedbackReviewConstants.AuthorNameMinLength))
            .Must(authorName => string.IsNullOrWhiteSpace(authorName)
                || authorName.Trim().Length <= FeedbackReviewConstants.AuthorNameMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateFeedbackReviewLocalizationDto.AuthorName),
                FeedbackReviewConstants.AuthorNameMaxLength));

        RuleFor(command => command.Localization.Text)
            .Must(text => !string.IsNullOrWhiteSpace(text))
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(CreateFeedbackReviewLocalizationDto.Text)))
            .Must(text => string.IsNullOrWhiteSpace(text)
                || text.Trim().Length >= FeedbackReviewConstants.TextMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateFeedbackReviewLocalizationDto.Text),
                FeedbackReviewConstants.TextMinLength))
            .Must(text => string.IsNullOrWhiteSpace(text)
                || text.Trim().Length <= FeedbackReviewConstants.TextMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateFeedbackReviewLocalizationDto.Text),
                FeedbackReviewConstants.TextMaxLength));
    }
}
