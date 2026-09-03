using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FeedbackReviews;

namespace VictoryCenter.BLL.Validators.FeedbackReviews;

public class BaseFeedbackReviewValidator : AbstractValidator<CreateFeedbackReviewDto>
{
    public BaseFeedbackReviewValidator()
    {
        RuleFor(dto => dto.Status)
            .IsInEnum()
            .WithMessage(ErrorMessagesConstants.UnknownStatusValue);

        RuleFor(dto => dto.AuthorName)
            .Must(authorName => !string.IsNullOrWhiteSpace(authorName))
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateFeedbackReviewDto.AuthorName)))
            .Must(authorName => string.IsNullOrWhiteSpace(authorName)
                || authorName.Trim().Length >= FeedbackReviewConstants.AuthorNameMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateFeedbackReviewDto.AuthorName),
                FeedbackReviewConstants.AuthorNameMinLength))
            .Must(authorName => string.IsNullOrWhiteSpace(authorName)
                || authorName.Trim().Length <= FeedbackReviewConstants.AuthorNameMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateFeedbackReviewDto.AuthorName),
                FeedbackReviewConstants.AuthorNameMaxLength));

        RuleFor(dto => dto.Text)
            .Must(text => !string.IsNullOrWhiteSpace(text))
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateFeedbackReviewDto.Text)))
            .Must(text => string.IsNullOrWhiteSpace(text)
                || text.Trim().Length >= FeedbackReviewConstants.TextMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateFeedbackReviewDto.Text),
                FeedbackReviewConstants.TextMinLength))
            .Must(text => string.IsNullOrWhiteSpace(text)
                || text.Trim().Length <= FeedbackReviewConstants.TextMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateFeedbackReviewDto.Text),
                FeedbackReviewConstants.TextMaxLength));
    }
}
