using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

namespace VictoryCenter.DAL.Entities.Localization;

public class HippotherapyLandingPageAnalysisSectionLocalization : LocalizationBase<HippotherapyLandingPageAnalysisSection>
{
    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;
}
