using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.ImpactStatistics.ReorderMetrics;
using VictoryCenter.BLL.Validators.MainPage.Dto;

namespace VictoryCenter.BLL.Validators.MainPage.Commands;

public class ReorderMetricsCommandValidator : AbstractValidator<ReorderMetricsCommand>
{
    public ReorderMetricsCommandValidator()
    {
        RuleFor(x => x.ReorderDto)
            .NotNull()
            .WithMessage(x => "Reorder metrics data cannot be null.")
            .SetValidator(new ReorderMetricsDtoValidator()!);
    }
}