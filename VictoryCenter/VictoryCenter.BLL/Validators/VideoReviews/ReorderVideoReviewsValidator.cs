using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.VideoReviews.Reorder;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.VideoReviews;

namespace VictoryCenter.BLL.Validators.VideoReviews;

public class ReorderVideoReviewsValidator : AbstractValidator<ReorderVideoReviewsCommand>
{
    public ReorderVideoReviewsValidator()
    {
        RuleFor(x => x.ReorderVideoReviewsDto.OrderedIds)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(ReorderVideoReviewsDto.OrderedIds)))
            .Must(ids => ids.Count > 0)
            .WithMessage(ErrorMessagesConstants.CollectionCannotBeEmpty(
                nameof(ReorderVideoReviewsDto.OrderedIds)))
            .Must(ids => ids.Count <= ReorderConstants.MaxElementsSwapCount)
            .WithMessage(ErrorMessagesConstants.CollectionCannotContainMoreThan(
                nameof(ReorderVideoReviewsDto.OrderedIds),
                ReorderConstants.MaxElementsSwapCount))
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(
                nameof(ReorderVideoReviewsDto.OrderedIds)));

        RuleForEach(x => x.ReorderVideoReviewsDto.OrderedIds)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustBePositive($"Each {nameof(ReorderVideoReviewsDto.OrderedIds)} element"));
    }
}
