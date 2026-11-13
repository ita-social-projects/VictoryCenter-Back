using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

namespace VictoryCenter.DAL.Entities.Localization;

public class HippotherapyLandingPageAdvantageCardLocalization : LocalizationBase<HippotherapyLandingPageAdvantageCard>
{
    [Audited]
    [RichText]
    public string Description { get; set; } = null!;
}
