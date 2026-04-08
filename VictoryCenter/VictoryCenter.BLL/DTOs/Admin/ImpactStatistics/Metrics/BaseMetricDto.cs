namespace VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;

public abstract record BaseMetricDto
{
    public string Value { get; init; } = null!;
    public string Signature { get; init; } = null!;
}
