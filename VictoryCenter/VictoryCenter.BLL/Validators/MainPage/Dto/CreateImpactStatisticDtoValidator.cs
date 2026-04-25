using FluentValidation;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Validators.MainPage.Dto;

public class CreateImpactStatisticDtoValidator
    : BaseImpactStatisticDtoValidator<CreateImpactStatisticDto, CreateMetricDto>
{
    public CreateImpactStatisticDtoValidator()
        : base(dto => dto.Metrics)
    {
        RuleForEach(x => x.Metrics)
            .SetValidator(new CreateMetricDtoValidator())
            .When(x => x.Metrics is not null);

        RuleForEach(x => x.Metrics)
            .Must(m => m is null || m.Type == MetricType.Raiced || m.Localization == null)
            .WithMessage("Localization is only allowed for the metric with type Raiced.");
    }
}
