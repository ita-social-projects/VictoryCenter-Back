namespace VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;

public record UpdateMetricVisibilityDto
{
    public bool IsHidden { get; init; }
}
