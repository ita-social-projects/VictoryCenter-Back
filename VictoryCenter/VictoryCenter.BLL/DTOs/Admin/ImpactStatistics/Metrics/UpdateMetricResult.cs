namespace VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;

public class UpdateMetricResult
{
    public bool WasModified { get; set; }
    public List<string> UpdatedFields { get; set; } = [];
}
