using VictoryCenter.DAL.Data.BaseEntity;

namespace VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

public class HippotherapyLandingPageAdvantageCard : BaseEntity
{
    public long AdvantagesSectionId { get; set; }

    public string Description { get; set; } = null!;

    public long? ImageId { get; set; }

    public Image? Image { get; set; }

    public long Priority { get; set; }

    public HippotherapyLandingPageAdvantagesSection AdvantagesSection { get; set; } = null!;
}
