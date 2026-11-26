using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;

namespace VictoryCenter.BLL.Validators.MainPage.Dto;

public class UpdateImpactStatisticDtoValidator
    : BaseImpactStatisticDtoValidator<UpdateImpactStatisticDto, UpdateMetricDto>
{
    public UpdateImpactStatisticDtoValidator()
        : base(dto => dto.Metrics)
    {
        RuleFor(x => x.Metrics)
            .Cascade(CascadeMode.Stop)
            .Must(metrics => metrics.Count <= MainPageConstants.ImpactStatistic.ExactMetricCount)
            .WithMessage(ErrorMessagesConstants.CollectionCannotContainMoreThan(
                nameof(UpdateImpactStatisticDto.Metrics), MainPageConstants.ImpactStatistic.ExactMetricCount))
            .Must(metrics => metrics.Where(metric => metric is not null).Select(metric => metric.Type).Distinct().Count()
                == metrics.Count(metric => metric is not null))
            .WithMessage(ErrorMessagesConstants.MetricTypesMustBeUnique())
            .When(x => x.Metrics is not null);

        RuleFor(x => x.Id)
            .GreaterThan(0)
            .When(x => x.Id.HasValue)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(UpdateImpactStatisticDto.Id)));

        RuleForEach(x => x.Metrics)
            .NotNull()
            .SetValidator(new UpdateMetricDtoValidator())
            .When(x => x.Metrics is not null);

        RuleFor(x => x.Metrics)
            .Must(metrics =>
            {
                if (metrics is null || metrics.Any(m => m is null))
                {
                    return true;
                }

                var ids = metrics
                    .Where(m => m.Id.HasValue)
                    .Select(m => m.Id!.Value)
                    .ToList();

                return ids.Distinct().Count() == ids.Count;
            })
            .WithMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(nameof(UpdateImpactStatisticDto.Metrics)))
            .When(x => x.Metrics is not null);
    }
}
