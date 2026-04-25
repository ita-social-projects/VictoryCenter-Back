using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage.Metrics;

namespace VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;

public record CreateMetricDto : BaseMetricDto
{
    public CreateMetricLocalizationDto? Localization { get; init; }
}
