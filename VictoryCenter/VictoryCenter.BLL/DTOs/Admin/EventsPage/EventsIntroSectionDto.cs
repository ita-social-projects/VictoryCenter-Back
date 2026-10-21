using VictoryCenter.BLL.DTOs.Admin.Localization.EventsIntroSection;

namespace VictoryCenter.BLL.DTOs.Admin.EventsPage;

public class EventsIntroSectionDto
{
    public long Id { get; set; }

    public required string EventsBlockTitle { get; set; }

    public required string PageDescription { get; set; }

    public bool IsEventsBlockTitleHidden { get; set; }

    public bool IsPageDescriptionHidden { get; set; }

    public List<EventsIntroSectionLocalizationDto> Localizations { get; set; } = [];
}
