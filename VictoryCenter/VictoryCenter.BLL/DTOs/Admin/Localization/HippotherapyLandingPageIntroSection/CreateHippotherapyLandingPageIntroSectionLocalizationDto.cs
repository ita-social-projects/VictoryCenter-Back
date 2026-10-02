using VictoryCenter.BLL.DTOs.Admin.Localization.Base;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageIntroSection;

public class CreateHippotherapyLandingPageIntroSectionLocalizationDto
    : UpdateHippotherapyLandingPageIntroSectionLocalizationDto, ILocalizationIdentity
{
    public long EntityId { get; init; }

    public long LanguageId { get; init; }
}
