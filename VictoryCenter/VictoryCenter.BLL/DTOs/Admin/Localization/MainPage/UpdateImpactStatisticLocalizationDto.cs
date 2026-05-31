namespace VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;

public record UpdateImpactStatisticLocalizationDto : BaseImpactStatisticLocalizationDto
{
    public long LanguageId { get; init; }
}
