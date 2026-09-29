using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.EventNews.Reorder;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.EventNews;

namespace VictoryCenter.BLL.Validators.EventNews;

public class ReorderEventNewsValidator : AbstractValidator<ReorderEventNewsCommand>
{
    public ReorderEventNewsValidator()
    {
        RuleFor(c => c.Dto.CategoryId)
            .GreaterThan(EventNewsConstants.ZeroCategoryId)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(
                nameof(ReorderEventNewsDto.CategoryId)));

        RuleFor(c => c.Dto.Ids)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(ReorderEventNewsDto.Ids)))
            .Must(ids => ids.Count > EventNewsConstants.ZeroCount)
            .WithMessage(ErrorMessagesConstants.CollectionCannotBeEmpty(
                nameof(ReorderEventNewsDto.Ids)))
            .Must(ids => ids.Count <= ReorderConstants.MaxElementsSwapCount)
            .WithMessage(c => ReorderConstants.ExceededMaxElementsSwapCount(c.Dto.Ids.Count))
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(
                nameof(ReorderEventNewsDto.Ids)));

        RuleForEach(c => c.Dto.Ids)
            .GreaterThan(EventNewsConstants.ZeroCategoryId)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(
                $"Each {nameof(ReorderEventNewsDto.Ids)} element."));
    }
}
