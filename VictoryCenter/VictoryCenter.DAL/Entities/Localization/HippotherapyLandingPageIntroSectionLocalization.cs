using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

namespace VictoryCenter.DAL.Entities.Localization;

public class HippotherapyLandingPageIntroSectionLocalization : LocalizationBase<HippotherapyLandingPageIntroSection>
{
    [Audited]
    [RichText]
    public string Title { get; set; } = null!;

    [Audited]
    [RichText]
    public string Description { get; set; } = null!;
}
