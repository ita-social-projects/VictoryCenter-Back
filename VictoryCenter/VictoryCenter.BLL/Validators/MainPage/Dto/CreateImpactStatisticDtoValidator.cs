using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics;

namespace VictoryCenter.BLL.Validators.MainPage.Dto;

public class CreateImpactStatisticDtoValidator : BaseImpactStatisticDtoValidator<CreateImpactStatisticDto>
{
    public CreateImpactStatisticDtoValidator()
    {
        RuleFor(x => x.Metrics)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateImpactStatisticDto.Metrics)))
            .Must(m => m.Count <= MainPageConstants.ImpactStatistic.MaxCount)
            .WithMessage(ErrorMessagesConstants.CollectionCannotContainMoreThan(
                nameof(CreateImpactStatisticDto.Metrics), MainPageConstants.ImpactStatistic.MaxCount));

        RuleForEach(x => x.Metrics)
            .SetValidator(new CreateMetricDtoValidator());
    }
}
