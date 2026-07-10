using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.DTOs.Public.EventNews;

public record PublishedEventNewsLocalizationDto
{
    public LocalizationInfoDto Language { get; init; } = null!;
    public string Title { get; init; } = null!;
    public string? Description { get; init; }
}
