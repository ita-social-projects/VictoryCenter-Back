using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage.Metrics;

namespace VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;

public record UpdateMetricDto : BaseMetricDto
{
    public long? Id { get; init; }
    public UpdateMetricLocalizationDto? Localization { get; init; }
}