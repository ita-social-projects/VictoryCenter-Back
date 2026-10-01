using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

namespace VictoryCenter.DAL.Entities.Localization;

public class HippotherapyLandingPageDescriptionSectionLocalization : LocalizationBase<HippotherapyLandingPageDescriptionSection>
{
    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;
}
