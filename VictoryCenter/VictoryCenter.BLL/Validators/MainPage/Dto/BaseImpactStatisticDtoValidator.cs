using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics;

namespace VictoryCenter.BLL.Validators.MainPage.Dto;

public abstract class BaseImpactStatisticDtoValidator<TDto> : AbstractValidator<TDto>
    where TDto : BaseImpactStatisticDto
{
    protected BaseImpactStatisticDtoValidator()
    {
        RuleFor(x => x.Description)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BaseImpactStatisticDto.Description)))
            .MinimumLength(MainPageConstants.ImpactStatistic.Description.MinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(BaseImpactStatisticDto.Description), MainPageConstants.ImpactStatistic.Description.MinLength))
            .MaximumLength(MainPageConstants.ImpactStatistic.Description.MaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseImpactStatisticDto.Description), MainPageConstants.ImpactStatistic.Description.MaxLength));
    }
}
