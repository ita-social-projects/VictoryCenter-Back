using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;

namespace VictoryCenter.BLL.Validators.MainPage.Dto;

public abstract class BaseMetricDtoValidator<TDto> : AbstractValidator<TDto>
    where TDto : BaseMetricDto
{
    protected BaseMetricDtoValidator()
    {
        RuleFor(x => x.Value)
            .GreaterThanOrEqualTo(0)
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BaseMetricDto.Value)));

        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BaseMetricDto.Name)))
            .MinimumLength(MainPageConstants.Metric.ValidationNameRules.MinLen)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(BaseMetricDto.Name), MainPageConstants.Metric.ValidationNameRules.MinLen))
            .MaximumLength(MainPageConstants.Metric.ValidationNameRules.MaxLen)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseMetricDto.Name), MainPageConstants.Metric.ValidationNameRules.MaxLen));

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BaseMetricDto.Type)));

        RuleFor(x => x.Prefix)
            .IsInEnum()
            .When(x => x.Prefix.HasValue)
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BaseMetricDto.Prefix)));
    }
}
