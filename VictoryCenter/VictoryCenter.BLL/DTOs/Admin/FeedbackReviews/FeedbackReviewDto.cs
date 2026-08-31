using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.FeedbackReviews;

public record FeedbackReviewDto
{
    public long Id { get; init; }
    public string AuthorName { get; init; } = null!;
    public string Text { get; init; } = null!;
    public Status Status { get; init; }
    public long Priority { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}
