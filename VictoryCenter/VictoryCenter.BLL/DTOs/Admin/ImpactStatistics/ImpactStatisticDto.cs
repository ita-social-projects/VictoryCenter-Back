using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;
using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.DTOs.Admin.ImpactStatistics;

public record ImpactStatisticDto
{
    public long Id { get; init; }
    public string Description { get; init; } = null!;
    public ImageDto? Image { get; init; } = null!;
    public ICollection<MetricDto> Metrics { get; init; } = [];
}
