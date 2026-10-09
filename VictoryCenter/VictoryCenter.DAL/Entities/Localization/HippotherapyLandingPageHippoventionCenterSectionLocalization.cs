using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

namespace VictoryCenter.DAL.Entities.Localization;

public class HippotherapyLandingPageHippoventionCenterSectionLocalization : LocalizationBase<HippotherapyLandingPageHippoventionCenterSection>
{
    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Pros { get; set; } = null!;
}
