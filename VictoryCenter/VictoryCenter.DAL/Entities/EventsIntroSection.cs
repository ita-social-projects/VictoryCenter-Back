using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Entities;

[Auditable]
public class EventsIntroSection : BaseEntity, ITranslatedEntity<EventsIntroSectionLocalization>
{
    [Audited]
    [RichText]
    public required string EventsBlockTitle { get; set; }

    [Audited]
    [RichText]
    public required string PageDescription { get; set; }

    [Audited]
    public bool IsEventsBlockTitleHidden { get; set; }

    [Audited]
    public bool IsPageDescriptionHidden { get; set; }

    public ICollection<EventsIntroSectionLocalization> Localizations { get; set; } = [];
}
