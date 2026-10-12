using VictoryCenter.DAL.Data.BaseEntity;

namespace VictoryCenter.DAL.Entities;

public class EventsIntroSection : BaseEntity
{
    public required string EventsBlockTitle { get; set; }

    public required string PageDescription { get; set; }

    public bool IsEventsBlockTitleHidden { get; set; }

    public bool IsPageDescriptionHidden { get; set; }
}
