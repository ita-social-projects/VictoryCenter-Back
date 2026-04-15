using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;

namespace VictoryCenter.BLL.DTOs.Admin.ImpactStatistics;

public record CreateImpactStatisticDto : BaseImpactStatisticDto
{
    public ICollection<CreateMetricDto> Metrics { get; init; } = [];
}
