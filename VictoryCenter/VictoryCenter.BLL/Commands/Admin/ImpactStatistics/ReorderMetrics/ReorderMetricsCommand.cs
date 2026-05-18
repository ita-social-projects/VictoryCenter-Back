using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;

namespace VictoryCenter.BLL.Commands.Admin.ImpactStatistics.ReorderMetrics;

public record ReorderMetricsCommand(ReorderMetricsDto ReorderDto)
    : IRequest<Result<Unit>>;