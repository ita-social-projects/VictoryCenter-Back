using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

namespace VictoryCenter.DAL.Entities.Localization;

public class HippotherapyLandingPageScientificReferenceLocalization : LocalizationBase<HippotherapyLandingPageScientificReference>
{
    [Audited]
    public string Name { get; set; } = null!;
}
