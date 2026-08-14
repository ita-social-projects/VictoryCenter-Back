using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.EventNews;

public record EventNewsDto
{
    public long Id { get; init; }
    public string? Slug { get; init; }
    public string? Resource { get; init; }
    public DateTimeOffset? PublishedAt { get; init; }
    public Status Status { get; init; }
    public ImageDto? PreviewImage { get; init; }
    public ImageDto? BackgroundImage { get; init; }
    public List<EventNewsCategoryShortDto> Categories { get; init; } = [];
    public List<EventNewsLocalizationDto> Localizations { get; init; } = [];
}
