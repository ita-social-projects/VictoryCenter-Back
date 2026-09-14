using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.FeedbackHistories;

public record FeedbackHistoryLocalizationDto
{
    public long EntityId { get; init; }
    public LocalizationInfoDto Language { get; init; } = null!;
    public string Title { get; init; } = null!;
    public string Story { get; init; } = null!;
    public TranslationStatus TranslationStatus { get; init; }
}
