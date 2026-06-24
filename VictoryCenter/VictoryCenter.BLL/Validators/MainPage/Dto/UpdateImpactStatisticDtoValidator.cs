using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics;

namespace VictoryCenter.BLL.Validators.MainPage.Dto;

public class UpdateImpactStatisticDtoValidator : AbstractValidator<UpdateImpactStatisticDto>
{
    public UpdateImpactStatisticDtoValidator()
    {
        RuleFor(x => x.Title)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateImpactStatisticDto.Title)))
            .MinimumLength(MainPageConstants.ImpactStatistic.ValidationTitleRules.MinLen)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdateImpactStatisticDto.Title), MainPageConstants.ImpactStatistic.ValidationTitleRules.MinLen))
            .MaximumLength(MainPageConstants.ImpactStatistic.ValidationTitleRules.MaxLen)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdateImpactStatisticDto.Title), MainPageConstants.ImpactStatistic.ValidationTitleRules.MaxLen));

        RuleFor(x => x.Id)
            .GreaterThan(0)
            .When(x => x.Id.HasValue)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(UpdateImpactStatisticDto.Id)));

        RuleFor(x => x.Metrics)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateImpactStatisticDto.Metrics)));

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