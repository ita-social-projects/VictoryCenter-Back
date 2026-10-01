namespace VictoryCenter.BLL.DTOs.Public.FeedbackReviews;

public record PublishedFeedbackReviewDto
{
    public long Id { get; init; }
    public string AuthorName { get; init; } = null!;
    public string Text { get; init; } = null!;
}
