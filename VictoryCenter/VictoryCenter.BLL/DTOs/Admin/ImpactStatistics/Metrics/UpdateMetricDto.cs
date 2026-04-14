namespace VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;

public record UpdateMetricDto : BaseMetricDto
{
    public long? Id { get; init; }
}