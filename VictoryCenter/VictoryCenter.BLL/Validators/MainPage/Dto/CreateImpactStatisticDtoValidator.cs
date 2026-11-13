using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;
using VictoryCenter.BLL.Validators.Localization.MainPage;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Validators.MainPage.Dto;

public class CreateImpactStatisticDtoValidator
    : BaseImpactStatisticDtoValidator<CreateImpactStatisticDto, CreateMetricDto>
{
    public CreateImpactStatisticDtoValidator()
        : base(dto => dto.Metrics)
    {
        RuleFor(x => x.Metrics)
            .Cascade(CascadeMode.Stop)
            .Must(metrics => metrics.Count == MainPageConstants.ImpactStatistic.ExactMetricCount)
            .WithMessage(ErrorMessagesConstants.MetricsMustContainExactlyNItems(MainPageConstants.ImpactStatistic.ExactMetricCount))
            .Must(metrics => Enum.GetValues<MetricType>().All(type => metrics.Any(metric => metric is not null && metric.Type == type)))
            .WithMessage(ErrorMessagesConstants.MetricTypesMustBeUnique())
            .When(x => x.Metrics is not null);

        RuleForEach(x => x.Metrics)
            .SetValidator(new CreateMetricDtoValidator())
            .When(x => x.Metrics is not null);

        When(x => x.Localization is not null, () =>
        {
            RuleFor(x => x.Localization!)
                .SetValidator(new CreateImpactStatisticLocalizationValidator());
        });
    }
}
