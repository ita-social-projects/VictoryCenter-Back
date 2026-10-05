using VictoryCenter.BLL.DTOs.Admin.Localization.VideoReviews;

namespace VictoryCenter.BLL.DTOs.Public.VideoReviews;

public record PublishedVideoReviewDto
{
    public long Id { get; init; }
    public string Title { get; init; } = null!;
    public string Link { get; init; } = null!;
    public List<VideoReviewLocalizationDto> Localizations { get; init; } = [];
}
