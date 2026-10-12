using VictoryCenter.BLL.DTOs.Public.EventNews;

namespace VictoryCenter.BLL.DTOs.Public.EventsPage;

public class PublicEventsPageDto
{
    public string EventsBlockTitle { get; set; } = string.Empty;
    public string PageDescription { get; set; } = string.Empty;
    public List<EventNewsCategoryDto> Categories { get; set; } = [];
}
