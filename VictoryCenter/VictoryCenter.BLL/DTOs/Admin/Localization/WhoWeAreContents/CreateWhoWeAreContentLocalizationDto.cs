namespace VictoryCenter.BLL.DTOs.Admin.Localization.WhoWeAreContents;

public record CreateWhoWeAreContentLocalizationDto : UpdateWhoWeAreContentLocalizationDto
{
    public long EntityId { get; init; }

    public long LanguageId { get; init; }
}
