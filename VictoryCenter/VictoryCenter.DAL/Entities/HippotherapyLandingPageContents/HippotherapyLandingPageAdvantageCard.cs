using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

[Auditable]
public class HippotherapyLandingPageAdvantageCard
    : BaseEntity, IGalleryCard, ITranslatedEntity<HippotherapyLandingPageAdvantageCardLocalization>
{
    [Audited]
    public long AdvantagesSectionId { get; set; }

    [Audited]
    [RichText]
    public string Description { get; set; } = null!;

    [Audited]
    public long? ImageId { get; set; }

    public Image? Image { get; set; }

    [Audited]
    public long Priority { get; set; }

    public HippotherapyLandingPageAdvantagesSection AdvantagesSection { get; set; } = null!;

    public ICollection<HippotherapyLandingPageAdvantageCardLocalization> Localizations { get; set; } = [];
}
