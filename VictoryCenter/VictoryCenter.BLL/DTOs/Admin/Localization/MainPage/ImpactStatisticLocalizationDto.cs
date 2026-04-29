namespace VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;

public record ImpactStatisticLocalizationDto : BaseImpactStatisticLocalizationDto
{
    public long EntityId { get; init; }
    public long LanguageId { get; init; }
}
