using VictoryCenter.DAL.Enums;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage.Metrics;

namespace VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;

public record UpdateSingleMetricDto
{
    public int? Value { get; init; }
    public string? Name { get; init; }
    public MetricType? Type { get; init; }
    public MetricPrefix? Prefix { get; init; }
    public bool? IsAutoSynced { get; init; }

    public UpdateMetricLocalizationDto? Localization { get; init; }

#pragma warning disable SA1011
    public byte[]? ExpectedVersion { get; init; }
#pragma warning restore SA1011
}
