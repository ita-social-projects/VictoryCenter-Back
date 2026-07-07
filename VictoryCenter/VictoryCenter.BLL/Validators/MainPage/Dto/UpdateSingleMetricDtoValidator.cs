using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;

namespace VictoryCenter.BLL.Validators.MainPage.Dto;

public class UpdateSingleMetricDtoValidator : AbstractValidator<UpdateSingleMetricDto>
{
    public UpdateSingleMetricDtoValidator()
    {
        RuleFor(x => x.Value)
            .GreaterThanOrEqualTo(0)
            .When(x => x.Value.HasValue);

        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired("Name"))
            .MinimumLength(MainPageConstants.Metric.Name.MinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters("Name", MainPageConstants.Metric.Name.MinLength))
            .MaximumLength(MainPageConstants.Metric.Name.MaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters("Name", MainPageConstants.Metric.Name.MaxLength))
            .When(x => x.Name != null);

        RuleFor(x => x.Type)
            .IsInEnum()
            .When(x => x.Type.HasValue);

        RuleFor(x => x.Prefix)
            .IsInEnum()
            .When(x => x.Prefix.HasValue);
    }
}
