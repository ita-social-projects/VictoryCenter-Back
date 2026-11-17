using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities.Localization;

public class VideoReviewLocalization : LocalizationBase<VideoReview>
{
    [Audited]
    public string Title { get; set; } = null!;
}
