using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.Validators.Common;

public class BaseReorderValidator<TReorderDto> : AbstractValidator<TReorderDto>
    where TReorderDto : BaseReorderDto
{
    public BaseReorderValidator(int? maxCount = null)
    {
        var ruleBuilder = RuleFor(x => x.OrderedIds)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.CollectionCannotBeEmpty(nameof(BaseReorderDto.OrderedIds)));

        if (maxCount.HasValue)
        {
            ruleBuilder
                .Must(ids => ids.Count <= maxCount.Value)
                .WithMessage(ErrorMessagesConstants.CollectionCannotContainMoreThan(
                    nameof(BaseReorderDto.OrderedIds),
                    maxCount.Value));
        }

        ruleBuilder
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(nameof(BaseReorderDto.OrderedIds)));

        RuleForEach(x => x.OrderedIds)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive($"Each ID in {nameof(BaseReorderDto.OrderedIds)}"));
    }
}
