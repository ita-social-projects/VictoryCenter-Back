using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

public class HippotherapyLandingPageParticipantCard
    : BaseEntity, IGalleryCard, ITranslatedEntity<HippotherapyLandingPageParticipantCardLocalization>
{
    public long ParticipantsSectionId { get; set; }

    public string Description { get; set; } = null!;

    public long? ImageId { get; set; }

    public Image? Image { get; set; }

    public long Priority { get; set; }

    public HippotherapyLandingPageParticipantsSection ParticipantsSection { get; set; } = null!;

    public ICollection<HippotherapyLandingPageParticipantCardLocalization> Localizations { get; set; } = [];
}
