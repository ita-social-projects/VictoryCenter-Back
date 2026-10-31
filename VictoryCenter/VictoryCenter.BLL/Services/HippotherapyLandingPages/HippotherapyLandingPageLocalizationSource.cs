using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAdvantagesSection;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAnalysisSection;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAnotherQuoteSection;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageDescriptionSection;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageEthicsSection;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageHippoventionCenterSection;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageHippoventionSection;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageIntroSection;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageParticipantsSection;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageQuoteSection;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageScientificReferencesSection;

namespace VictoryCenter.BLL.Services.HippotherapyLandingPages;

public sealed record HippotherapyLandingPageLocalizationSource(
    IReadOnlyList<HippotherapyLandingPageIntroSectionLocalizationDto> Intro,
    IReadOnlyList<HippotherapyLandingPageDescriptionSectionLocalizationDto> Description,
    IReadOnlyList<HippotherapyLandingPageQuoteSectionLocalizationDto> Quote,
    IReadOnlyList<HippotherapyLandingPageHippoventionSectionLocalizationDto> Hippovention,
    IReadOnlyList<HippotherapyLandingPageHippoventionCenterSectionLocalizationDto> HippoventionCenter,
    IReadOnlyList<HippotherapyLandingPageAdvantagesSectionLocalizationDto> Advantages,
    IReadOnlyList<HippotherapyLandingPageAnalysisSectionLocalizationDto> Analysis,
    IReadOnlyList<HippotherapyLandingPageScientificReferencesSectionLocalizationDto> ScientificReferences,
    IReadOnlyList<HippotherapyLandingPageAnotherQuoteSectionLocalizationDto> AnotherQuote,
    IReadOnlyList<HippotherapyLandingPageParticipantsSectionLocalizationDto> Participants,
    IReadOnlyList<HippotherapyLandingPageEthicsSectionLocalizationDto> Ethics)
{
    public static HippotherapyLandingPageLocalizationSource Empty { get; } =
        new([], [], [], [], [], [], [], [], [], [], []);
}
