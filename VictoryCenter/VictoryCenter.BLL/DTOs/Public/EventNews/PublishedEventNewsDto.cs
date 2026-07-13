using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.DTOs.Public.EventNews;

public record PublishedEventNewsDto
{
    public long Id { get; init; }
    public string? Slug { get; init; }
    public string? Resource { get; init; }
    public DateTimeOffset? PublishedAt { get; init; }
    public ImageDto? PreviewImage { get; init; }
    public List<EventNewsCategoryDto> Categories { get; init; } = [];
    public List<PublishedEventNewsLocalizationDto> Localizations { get; init; } = [];
}
