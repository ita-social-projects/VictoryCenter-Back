using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Entities;

public class EventsIntroSection : BaseEntity, ITranslatedEntity<EventsIntroSectionLocalization>
{
    [RichText]
    public required string EventsBlockTitle { get; set; }

    [RichText]
    public required string PageDescription { get; set; }

    public bool IsEventsBlockTitleHidden { get; set; }

    public bool IsPageDescriptionHidden { get; set; }

    public ICollection<EventsIntroSectionLocalization> Localizations { get; set; } = [];
}
