namespace VictoryCenter.BLL.DTOs.Admin.Localization.FeedbackReviews;

public record CreateFeedbackReviewLocalizationDto
{
    public long EntityId { get; init; }
    public long LanguageId { get; init; }
    public string AuthorName { get; init; } = null!;
    public string Text { get; init; } = null!;
}
