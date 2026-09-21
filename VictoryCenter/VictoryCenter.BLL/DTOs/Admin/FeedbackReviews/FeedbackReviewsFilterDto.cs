using VictoryCenter.BLL.DTOs.Admin.Localization.Base;
using VictoryCenter.BLL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.FeedbackReviews;

public record FeedbackReviewsFilterDto : ITranslationStatusFilterDto
{
    public int? Offset { get; init; }
    public int? Limit { get; init; }
    public TranslationStatusFilter? TranslationStatusFilter { get; set; }
}
