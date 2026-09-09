namespace VictoryCenter.DAL.Entities.Localization;

public class FeedbackReviewLocalization : LocalizationBase<FeedbackReview>
{
    public string AuthorName { get; set; } = null!;

    public string Text { get; set; } = null!;
}
