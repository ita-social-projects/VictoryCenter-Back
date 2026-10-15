using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

namespace VictoryCenter.DAL.Entities.Localization;

public class HippotherapyLandingPageEthicsSectionLocalization : LocalizationBase<HippotherapyLandingPageEthicsSection>
{
    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;
}
