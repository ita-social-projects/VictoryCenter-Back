namespace VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;

public record MetricDto
{
    public long Id { get; init; }

    public string Value { get; set; } = null!;
    public string Signature { get; init; } = null!;
}
