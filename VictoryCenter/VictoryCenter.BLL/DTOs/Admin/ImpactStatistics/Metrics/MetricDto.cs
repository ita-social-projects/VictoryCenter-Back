using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage.Metrics;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;

public record MetricDto
{
    public long Id { get; init; }
    public int Value { get; init; }
    public string Name { get; init; } = null!;
    public MetricType Type { get; init; }
    public MetricPrefix? Prefix { get; init; }
    public bool IsHidden { get; init; }
    public ICollection<MetricLocalizationDto> Localizations { get; init; } = [];
}
