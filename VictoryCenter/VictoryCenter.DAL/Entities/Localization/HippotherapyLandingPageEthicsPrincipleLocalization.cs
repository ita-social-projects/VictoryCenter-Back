using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

namespace VictoryCenter.DAL.Entities.Localization;

public class HippotherapyLandingPageEthicsPrincipleLocalization : LocalizationBase<HippotherapyLandingPageEthicsPrinciple>
{
    [RichText]
    public string Text { get; set; } = null!;
}
