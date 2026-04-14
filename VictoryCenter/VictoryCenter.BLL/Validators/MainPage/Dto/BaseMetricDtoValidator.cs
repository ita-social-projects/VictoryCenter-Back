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
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BaseMetricDto.Value)))
            .MinimumLength(MainPageConstants.Metric.Value.MinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(BaseMetricDto.Value), MainPageConstants.Metric.Value.MinLength))
            .MaximumLength(MainPageConstants.Metric.Value.MaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseMetricDto.Value), MainPageConstants.Metric.Value.MaxLength));

        RuleFor(x => x.Signature)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BaseMetricDto.Signature)))
            .MinimumLength(MainPageConstants.Metric.Signature.MinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(BaseMetricDto.Signature), MainPageConstants.Metric.Signature.MinLength))
            .MaximumLength(MainPageConstants.Metric.Signature.MaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseMetricDto.Signature), MainPageConstants.Metric.Signature.MaxLength));
    }
}
