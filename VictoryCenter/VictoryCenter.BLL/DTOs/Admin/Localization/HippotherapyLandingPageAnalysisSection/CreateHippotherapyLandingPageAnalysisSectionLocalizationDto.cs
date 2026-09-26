using VictoryCenter.BLL.DTOs.Admin.Localization.Base;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAnalysisSection;

public class CreateHippotherapyLandingPageAnalysisSectionLocalizationDto
    : UpdateHippotherapyLandingPageAnalysisSectionLocalizationDto, ILocalizationIdentity
{
    public long EntityId { get; init; }

    public long LanguageId { get; init; }
}
