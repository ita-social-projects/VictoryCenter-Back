using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

[Auditable]
public class HippotherapyLandingPageParticipantCard
    : BaseEntity, IGalleryCard, ITranslatedEntity<HippotherapyLandingPageParticipantCardLocalization>
{
    [Audited]
    public long ParticipantsSectionId { get; set; }

    [Audited]
    [RichText]
    public string Description { get; set; } = null!;

    [Audited]
    public long? ImageId { get; set; }

    public Image? Image { get; set; }

    [Audited]
    public long Priority { get; set; }

    public HippotherapyLandingPageParticipantsSection ParticipantsSection { get; set; } = null!;

    public ICollection<HippotherapyLandingPageParticipantCardLocalization> Localizations { get; set; } = [];
}
