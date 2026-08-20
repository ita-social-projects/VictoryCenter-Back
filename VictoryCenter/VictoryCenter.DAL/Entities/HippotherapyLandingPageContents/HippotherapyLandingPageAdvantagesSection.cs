using VictoryCenter.DAL.Data.BaseEntity;

namespace VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

public class HippotherapyLandingPageAdvantagesSection : BaseEntity
{
    public long HippotherapyLandingPageId { get; set; }

    public string Title { get; set; } = null!;

    public ICollection<HippotherapyLandingPageAdvantageCard> AdvantageCards { get; set; } = [];

    public HippotherapyLandingPage HippotherapyLandingPage { get; set; } = null!;
}
