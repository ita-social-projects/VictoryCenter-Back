using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.VideoReviews;

public record VideoReviewLocalizationDto
{
    public long EntityId { get; init; }
    public LocalizationInfoDto Language { get; init; } = null!;
    public string Title { get; init; } = null!;
    public TranslationStatus TranslationStatus { get; init; }
}
