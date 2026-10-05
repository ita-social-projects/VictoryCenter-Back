using VictoryCenter.BLL.DTOs.Admin.Localization.Base;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageHippoventionCenterSection;

public record DeleteHippotherapyLandingPageHippoventionCenterSectionLocalizationDto : ILocalizationIdentity
{
    public long EntityId { get; init; }

    public long LanguageId { get; init; }
}
