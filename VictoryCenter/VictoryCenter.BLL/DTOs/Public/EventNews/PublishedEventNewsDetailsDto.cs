using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.DTOs.Public.EventNews;
public record PublishedEventNewsDetailsDto : PublishedEventNewsDto
{
    public ImageDto? BackgroundImage { get; init; }
}
