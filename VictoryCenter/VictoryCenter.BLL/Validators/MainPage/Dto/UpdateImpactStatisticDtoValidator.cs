using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics;

namespace VictoryCenter.BLL.Validators.MainPage.Dto;

public class UpdateImpactStatisticDtoValidator : BaseImpactStatisticDtoValidator<UpdateImpactStatisticDto>
{
    public UpdateImpactStatisticDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .When(x => x.Id.HasValue)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(UpdateImpactStatisticDto.Id)));

        RuleFor(x => x.Metrics)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateImpactStatisticDto.Metrics)))
            .Must(m => m.Count <= MainPageConstants.ImpactStatistic.MaxCount)
            .WithMessage(ErrorMessagesConstants.CollectionCannotContainMoreThan(
                nameof(UpdateImpactStatisticDto.Metrics), MainPageConstants.ImpactStatistic.MaxCount));

        RuleForEach(x => x.Metrics)
            .NotNull()
            .SetValidator(new UpdateMetricDtoValidator());

        RuleFor(x => x.Metrics)
            .Must(metrics =>
            {
                if (metrics is null)
                {
                    return true;
                }

                if (metrics.Any(m => m is null))
                {
                    return true;
                }

                var ids = metrics
                    .Where(m => m!.Id.HasValue)
                    .Select(m => m!.Id!.Value)
                    .ToList();

                return ids.Distinct().Count() == ids.Count;
            })
            .WithMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(nameof(UpdateImpactStatisticDto.Metrics)));
    }
}