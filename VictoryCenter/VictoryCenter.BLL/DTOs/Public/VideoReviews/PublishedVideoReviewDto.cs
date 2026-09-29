namespace VictoryCenter.BLL.DTOs.Public.VideoReviews;

public record PublishedVideoReviewDto
{
    public long Id { get; init; }
    public string Title { get; init; } = null!;
    public string Link { get; init; } = null!;
}
