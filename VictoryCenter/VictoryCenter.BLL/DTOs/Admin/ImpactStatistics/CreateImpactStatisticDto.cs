using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;

namespace VictoryCenter.BLL.DTOs.Admin.ImpactStatistics;

public record CreateImpactStatisticDto : BaseImpactStatisticDto
{
    public ICollection<CreateMetricDto> Metrics { get; init; } = [];
    public CreateImpactStatisticLocalizationDto? Localization { get; init; }
}
