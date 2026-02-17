using VictoryCenter.BLL.DTOs.Admin.Localization.Base;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.WhoWeAreContents;

public record CreateWhoWeAreContentLocalizationDto : UpdateWhoWeAreContentLocalizationDto, ILocalizationIdentity
{
    public long EntityId { get; init; }

    public long LanguageId { get; init; }
}
