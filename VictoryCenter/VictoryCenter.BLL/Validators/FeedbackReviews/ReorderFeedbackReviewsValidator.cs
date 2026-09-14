using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.FeedbackReviews.Reorder;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FeedbackReviews;

namespace VictoryCenter.BLL.Validators.FeedbackReviews;

public class ReorderFeedbackReviewsValidator : AbstractValidator<ReorderFeedbackReviewsCommand>
{
    public ReorderFeedbackReviewsValidator()
    {
        RuleFor(x => x.ReorderFeedbackReviewsDto.OrderedIds)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(ReorderFeedbackReviewsDto.OrderedIds)))
            .Must(ids => ids.Count > 0)
            .WithMessage(ErrorMessagesConstants.CollectionCannotBeEmpty(
                nameof(ReorderFeedbackReviewsDto.OrderedIds)))
            .Must(ids => ids.Count <= ReorderConstants.MaxElementsSwapCount)
            .WithMessage(ErrorMessagesConstants.CollectionCannotContainMoreThan(
                nameof(ReorderFeedbackReviewsDto.OrderedIds),
                ReorderConstants.MaxElementsSwapCount))
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(
                nameof(ReorderFeedbackReviewsDto.OrderedIds)));

        RuleForEach(x => x.ReorderFeedbackReviewsDto.OrderedIds)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustBePositive($"Each {nameof(ReorderFeedbackReviewsDto.OrderedIds)} element"));
    }
}