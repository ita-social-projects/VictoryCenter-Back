namespace VictoryCenter.BLL.DTOs.Admin.FeedbackReviews;

public record FeedbackReviewsFilterDto
{
    public int? Offset { get; init; }
    public int? Limit { get; init; }
}
