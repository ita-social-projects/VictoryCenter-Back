namespace VictoryCenter.BLL.DTOs.Admin.EventsPage;

public class EventsIntroSectionDto
{
    public required string EventsBlockTitle { get; set; }

    public required string PageDescription { get; set; }

    public bool IsEventsBlockTitleHidden { get; set; }

    public bool IsPageDescriptionHidden { get; set; }
}
