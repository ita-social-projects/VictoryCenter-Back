using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;
using VictoryCenter.BLL.Validators.Common;

namespace VictoryCenter.BLL.Validators.MainPage.Dto;

public class ReorderMetricsDtoValidator : BaseReorderValidator<ReorderMetricsDto>
{
    public ReorderMetricsDtoValidator()
        : base(MainPageConstants.ExactMetricCount)
    {
        RuleFor(x => x.StatisticId)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(ReorderMetricsDto.StatisticId)));
    }
}