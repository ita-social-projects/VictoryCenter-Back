using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

namespace VictoryCenter.DAL.Entities.Localization;

public class HippotherapyLandingPageAdvantagesSectionLocalization : LocalizationBase<HippotherapyLandingPageAdvantagesSection>
{
    [Audited]
    [RichText]
    public string Title { get; set; } = null!;
}
