using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.FeedbackHistories.Reorder;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FeedbackHistories;

namespace VictoryCenter.BLL.Validators.FeedbackHistories;

public class ReorderFeedbackHistoriesValidator : AbstractValidator<ReorderFeedbackHistoriesCommand>
{
    public ReorderFeedbackHistoriesValidator()
    {
        RuleFor(x => x.ReorderFeedbackHistoriesDto.OrderedIds)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(ReorderFeedbackHistoriesDto.OrderedIds)))
            .Must(ids => ids.Count > 0)
            .WithMessage(ErrorMessagesConstants.CollectionCannotBeEmpty(
                nameof(ReorderFeedbackHistoriesDto.OrderedIds)))
            .Must(ids => ids.Count <= ReorderConstants.MaxElementsSwapCount)
            .WithMessage(ErrorMessagesConstants.CollectionCannotContainMoreThan(
                nameof(ReorderFeedbackHistoriesDto.OrderedIds),
                ReorderConstants.MaxElementsSwapCount))
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(
                nameof(ReorderFeedbackHistoriesDto.OrderedIds)));

        RuleForEach(x => x.ReorderFeedbackHistoriesDto.OrderedIds)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustBePositive($"Each {nameof(ReorderFeedbackHistoriesDto.OrderedIds)} element"));
    }
}
