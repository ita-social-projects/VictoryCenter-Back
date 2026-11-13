using System.Linq.Expressions;
using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;

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
            .MinimumLength(MainPageConstants.ImpactStatistic.ValidationTitleRules.MinLen)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                "Title", MainPageConstants.ImpactStatistic.ValidationTitleRules.MinLen))
            .MaximumLength(MainPageConstants.ImpactStatistic.ValidationTitleRules.MaxLen)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                "Title", MainPageConstants.ImpactStatistic.ValidationTitleRules.MaxLen));

        RuleFor(metricsSelector)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired("Metrics"));

        RuleForEach(enumerableSelector)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired("Metrics[]"));
    }
}
