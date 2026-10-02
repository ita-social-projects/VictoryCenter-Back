namespace VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;

public class UpdateMetricResult
{
    public bool WasModified { get; set; }
    public List<string> UpdatedFields { get; set; } = [];

#pragma warning disable SA1011
    public byte[]? RowVersion { get; set; }
#pragma warning restore SA1011
    public int? Value { get; set; }
    public string? LocalizationValue { get; set; }
}
