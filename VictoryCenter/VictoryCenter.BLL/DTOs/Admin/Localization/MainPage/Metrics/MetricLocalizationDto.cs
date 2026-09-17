using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.MainPage.Metrics;

public record MetricLocalizationDto : BaseMetricLocalizationDto
{
    public long EntityId { get; init; }
    public long LanguageId { get; init; }
    public LocalizationInfoDto LocalizationInfoDto { get; init; } = null!;
}
