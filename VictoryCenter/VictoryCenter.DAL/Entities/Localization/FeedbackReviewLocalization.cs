using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities.Localization;

public class FeedbackReviewLocalization : LocalizationBase<FeedbackReview>
{
    [Audited]
    public string AuthorName { get; set; } = null!;

    [Audited]
    public string Text { get; set; } = null!;
}
