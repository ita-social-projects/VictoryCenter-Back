using VictoryCenter.BLL.DTOs.Admin.Localization.Base;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageScientificReferencesSection;

public class CreateHippotherapyLandingPageScientificReferencesSectionLocalizationDto
    : UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationDto, ILocalizationIdentity
{
    public long EntityId { get; init; }

    public long LanguageId { get; init; }
}
