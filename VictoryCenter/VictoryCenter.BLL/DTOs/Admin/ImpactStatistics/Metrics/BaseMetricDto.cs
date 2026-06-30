using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;

public abstract record BaseMetricDto
{
    public int Value { get; init; }
    public string Name { get; init; } = null!;
    public MetricType Type { get; init; }
    public MetricPrefix? Prefix { get; init; }
    public bool IsAutoSynced { get; init; }
}
