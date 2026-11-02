using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.WhoWeAreContents;

namespace VictoryCenter.DAL.Entities.Localization;

public class WhoWeAreContentLocalization : LocalizationBase<WhoWeAreContent>
{
    [RichText]
    public string? Title { get; set; }

    [RichText]
    public string? Description { get; set; }
}
