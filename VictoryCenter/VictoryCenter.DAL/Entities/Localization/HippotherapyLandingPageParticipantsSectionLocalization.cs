using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

namespace VictoryCenter.DAL.Entities.Localization;

public class HippotherapyLandingPageParticipantsSectionLocalization : LocalizationBase<HippotherapyLandingPageParticipantsSection>
{
    [Audited]
    [RichText]
    public string Title { get; set; } = null!;
}
