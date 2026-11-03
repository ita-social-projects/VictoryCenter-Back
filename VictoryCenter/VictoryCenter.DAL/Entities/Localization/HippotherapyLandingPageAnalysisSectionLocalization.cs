using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

namespace VictoryCenter.DAL.Entities.Localization;

public class HippotherapyLandingPageAnalysisSectionLocalization : LocalizationBase<HippotherapyLandingPageAnalysisSection>
{
    [RichText]
    public string Title { get; set; } = null!;

    [RichText]
    public string Description { get; set; } = null!;
}
