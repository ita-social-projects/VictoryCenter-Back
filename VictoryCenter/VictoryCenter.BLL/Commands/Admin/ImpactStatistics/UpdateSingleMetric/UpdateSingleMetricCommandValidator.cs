using FluentValidation;
using VictoryCenter.BLL.Validators.MainPage.Dto;

namespace VictoryCenter.BLL.Commands.Admin.ImpactStatistics.UpdateSingleMetric;

public class UpdateSingleMetricCommandValidator : AbstractValidator<UpdateSingleMetricCommand>
{
    public UpdateSingleMetricCommandValidator()
    {
        RuleFor(x => x.MetricId)
            .GreaterThan(0);

        RuleFor(x => x.Dto)
            .NotNull()
            .SetValidator(new UpdateSingleMetricDtoValidator());
    }
}
