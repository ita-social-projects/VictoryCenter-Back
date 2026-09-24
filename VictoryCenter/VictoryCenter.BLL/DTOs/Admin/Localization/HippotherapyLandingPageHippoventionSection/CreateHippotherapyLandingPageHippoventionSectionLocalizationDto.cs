using VictoryCenter.BLL.DTOs.Admin.Localization.Base;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageHippoventionSection;

public class CreateHippotherapyLandingPageHippoventionSectionLocalizationDto
    : UpdateHippotherapyLandingPageHippoventionSectionLocalizationDto, ILocalizationIdentity
{
    public long EntityId { get; init; }

    public long LanguageId { get; init; }
}
