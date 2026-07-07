using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;

namespace VictoryCenter.BLL.Commands.Admin.ImpactStatistics.UpdateSingleMetric;

public record UpdateSingleMetricCommand(long MetricId, UpdateSingleMetricDto Dto) : IRequest<Result<Unit>>;
