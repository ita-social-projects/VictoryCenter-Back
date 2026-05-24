using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;

public record ReorderMetricsDto : BaseReorderDto
{
    public long StatisticId { get; init; }
}