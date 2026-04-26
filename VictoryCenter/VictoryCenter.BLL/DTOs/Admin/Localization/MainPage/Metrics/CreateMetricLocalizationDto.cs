namespace VictoryCenter.BLL.DTOs.Admin.Localization.MainPage.Metrics;

public record CreateMetricLocalizationDto : BaseMetricLocalizationDto
{
    public long LanguageId { get; init; }
}
