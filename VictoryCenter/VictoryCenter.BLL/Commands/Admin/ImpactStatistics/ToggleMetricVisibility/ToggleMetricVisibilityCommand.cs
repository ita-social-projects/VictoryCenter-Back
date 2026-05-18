using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;

namespace VictoryCenter.BLL.Commands.Admin.ImpactStatistics.ToggleMetricVisibility;

public record ToggleMetricVisibilityCommand(long MetricId, UpdateMetricVisibilityDto Dto)
    : IRequest<Result<Unit>>;
