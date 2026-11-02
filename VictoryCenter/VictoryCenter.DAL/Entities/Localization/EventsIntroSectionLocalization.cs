using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities.Localization;

public class EventsIntroSectionLocalization : LocalizationBase<EventsIntroSection>
{
    [RichText]
    public string? EventsBlockTitle { get; set; }

    [RichText]
    public string? PageDescription { get; set; }
}
