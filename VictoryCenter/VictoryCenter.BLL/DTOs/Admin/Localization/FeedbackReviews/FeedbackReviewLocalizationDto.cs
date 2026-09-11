using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.FeedbackReviews;

public record FeedbackReviewLocalizationDto
{
    public long EntityId { get; init; }
    public LocalizationInfoDto Language { get; init; } = null!;
    public string AuthorName { get; init; } = null!;
    public string Text { get; init; } = null!;
    public TranslationStatus TranslationStatus { get; init; }
}
