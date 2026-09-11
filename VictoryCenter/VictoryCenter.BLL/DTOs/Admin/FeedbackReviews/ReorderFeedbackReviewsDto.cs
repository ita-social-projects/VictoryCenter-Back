namespace VictoryCenter.BLL.DTOs.Admin.FeedbackReviews;

public record ReorderFeedbackReviewsDto
{
    public List<long> OrderedIds { get; init; } = [];
}
