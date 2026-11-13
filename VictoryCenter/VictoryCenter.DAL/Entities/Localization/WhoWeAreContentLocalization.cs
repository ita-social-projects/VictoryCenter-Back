using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.WhoWeAreContents;

namespace VictoryCenter.DAL.Entities.Localization;

public class WhoWeAreContentLocalization : LocalizationBase<WhoWeAreContent>
{
    [Audited]
    [RichText]
    public string? Title { get; set; }

    [Audited]
    [RichText]
    public string? Description { get; set; }
}
