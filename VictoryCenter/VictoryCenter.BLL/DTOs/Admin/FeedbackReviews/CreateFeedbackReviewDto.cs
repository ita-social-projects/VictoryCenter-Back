using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.FeedbackReviews;

public record CreateFeedbackReviewDto
{
    public string AuthorName { get; init; } = null!;
    public string Text { get; init; } = null!;
    public Status Status { get; init; }
}
