using VictoryCenter.BLL.DTOs.Public.EventNews;

namespace VictoryCenter.BLL.DTOs.Public.EventsPage;

public class PublicEventsPageDto
{
    public string EventsBlockTitle { get; set; } = string.Empty;
    public string PageDescription { get; set; } = string.Empty;
    public bool IsEventsBlockTitleHidden { get; set; }
    public bool IsPageDescriptionHidden { get; set; }
    public List<PublicEventsIntroSectionLocalizationDto> Localizations { get; set; } = [];
    public List<EventNewsCategoryDto> Categories { get; set; } = [];
}
