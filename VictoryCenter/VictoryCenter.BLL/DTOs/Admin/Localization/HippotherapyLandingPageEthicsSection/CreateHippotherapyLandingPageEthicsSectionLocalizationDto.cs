using VictoryCenter.BLL.DTOs.Admin.Localization.Base;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageEthicsSection;

public class CreateHippotherapyLandingPageEthicsSectionLocalizationDto
    : UpdateHippotherapyLandingPageEthicsSectionLocalizationDto, ILocalizationIdentity
{
    public long EntityId { get; init; }

    public long LanguageId { get; init; }
}
