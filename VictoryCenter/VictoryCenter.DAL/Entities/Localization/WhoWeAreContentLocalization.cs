using VictoryCenter.DAL.Entities.WhoWeAreContents;

namespace VictoryCenter.DAL.Entities.Localization;

public class WhoWeAreContentLocalization : LocalizationBase<WhoWeAreContent>
{
    public string? Title { get; set; }

    public string? Description { get; set; }
}
