using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;

namespace VictoryCenter.BLL.DTOs.Admin.ImpactStatistics;

public record UpdateImpactStatisticDto : BaseImpactStatisticDto
{
    public long? Id { get; init; }
    public ICollection<UpdateMetricDto> Metrics { get; init; } = [];
}