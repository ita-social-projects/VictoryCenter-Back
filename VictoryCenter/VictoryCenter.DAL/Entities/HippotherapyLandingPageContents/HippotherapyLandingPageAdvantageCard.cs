using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Interfaces;

namespace VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

public class HippotherapyLandingPageAdvantageCard : BaseEntity, IGalleryCard
{
    public long AdvantagesSectionId { get; set; }

    public string Description { get; set; } = null!;

    public long? ImageId { get; set; }

    public Image? Image { get; set; }

    public long Priority { get; set; }

    public HippotherapyLandingPageAdvantagesSection AdvantagesSection { get; set; } = null!;
}
