namespace VictoryCenter.BLL.DTOs.Admin.Localization.VideoReviews;

public record CreateVideoReviewLocalizationDto
{
    public long EntityId { get; init; }
    public long LanguageId { get; init; }
    public string Title { get; init; } = null!;
}
