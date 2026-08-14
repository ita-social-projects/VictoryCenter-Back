using VictoryCenter.DAL.Data.BaseEntity;

namespace VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

public class HippotherapyLandingPageParticipantCard : BaseEntity
{
    public long ParticipantsSectionId { get; set; }

    public string Description { get; set; } = null!;

    public long? ImageId { get; set; }

    public Image? Image { get; set; }

    public long Priority { get; set; }

    public HippotherapyLandingPageParticipantsSection ParticipantsSection { get; set; } = null!;
}
