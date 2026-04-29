using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;

namespace VictoryCenter.BLL.DTOs.Admin.ImpactStatistics;

public record UpdateImpactStatisticDto : BaseImpactStatisticDto
{
    public long? Id { get; init; }
    public ICollection<UpdateMetricDto> Metrics { get; init; } = [];
    public UpdateImpactStatisticLocalizationDto? Localization { get; init; }
}