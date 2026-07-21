using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.EventNews;

public record CreateEventNewsDto
{
    public string? Resource { get; init; }
    public DateTimeOffset? PublishedAt { get; init; }
    public Status Status { get; init; }
    public long? PreviewImageId { get; init; }
    public long? BackgroundImageId { get; init; }
    public List<long> CategoryIds { get; init; } = [];
    public List<CreateEventNewsLocalizationDto> Localizations { get; init; } = [];
}
