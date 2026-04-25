using System.Linq.Expressions;
using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Validators.MainPage.Dto;

public abstract class BaseImpactStatisticDtoValidator<TDto, TMetric> : AbstractValidator<TDto>
    where TDto : BaseImpactStatisticDto
    where TMetric : BaseMetricDto
{
    protected BaseImpactStatisticDtoValidator(Expression<Func<TDto, ICollection<TMetric>>> metricsSelector)
    {
        var enumerableSelector = Expression.Lambda<Func<TDto, IEnumerable<TMetric>>>(
            metricsSelector.Body, metricsSelector.Parameters);

        RuleFor(x => x.Title)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired("Title"))
            .MinimumLength(MainPageConstants.ImpactStatistic.Title.MinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                "Title", MainPageConstants.ImpactStatistic.Title.MinLength))
            .MaximumLength(MainPageConstants.ImpactStatistic.Title.MaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                "Title", MainPageConstants.ImpactStatistic.Title.MaxLength));

        RuleFor(metricsSelector)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired("Metrics"))
            .Must(m => m.Count == MainPageConstants.ImpactStatistic.ExactMetricCount)
            .WithMessage($"Metrics must contain exactly {MainPageConstants.ImpactStatistic.ExactMetricCount} items.")
            .Must(HasAllUniqueMetricTypes)
            .WithMessage("Each of the four metric types must appear exactly once: Partners, Programs, Raiced, TherapyHours.");

        RuleForEach(enumerableSelector)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired("Metrics[]"));
    }

    private static bool HasAllUniqueMetricTypes(ICollection<TMetric> metrics)
        => Enum.GetValues<MetricType>().All(t => metrics.Any(m => m is not null && m.Type == t));
}
