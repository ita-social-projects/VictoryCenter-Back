namespace VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;

public record CreateImpactStatisticLocalizationDto : BaseImpactStatisticLocalizationDto
{
    public long LanguageId { get; init; }
}
