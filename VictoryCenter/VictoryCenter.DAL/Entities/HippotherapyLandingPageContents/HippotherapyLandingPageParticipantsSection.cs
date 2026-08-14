using VictoryCenter.DAL.Data.BaseEntity;

namespace VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

public class HippotherapyLandingPageParticipantsSection : BaseEntity
{
    public long HippotherapyLandingPageId { get; set; }

    public string Title { get; set; } = null!;

    public ICollection<HippotherapyLandingPageParticipantCard> ParticipantCards { get; set; } = [];

    public HippotherapyLandingPage HippotherapyLandingPage { get; set; } = null!;
}
