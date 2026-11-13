using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities.Localization;

public class EventsIntroSectionLocalization : LocalizationBase<EventsIntroSection>
{
    [Audited]
    [RichText]
    public string? EventsBlockTitle { get; set; }

    [Audited]
    [RichText]
    public string? PageDescription { get; set; }
}
