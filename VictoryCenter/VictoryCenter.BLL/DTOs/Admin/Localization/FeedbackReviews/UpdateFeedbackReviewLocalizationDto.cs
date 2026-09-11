namespace VictoryCenter.BLL.DTOs.Admin.Localization.FeedbackReviews;

public record UpdateFeedbackReviewLocalizationDto
{
    public string AuthorName { get; init; } = null!;
    public string Text { get; init; } = null!;
}
