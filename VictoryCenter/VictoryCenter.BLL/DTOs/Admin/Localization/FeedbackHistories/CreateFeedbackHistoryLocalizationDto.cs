namespace VictoryCenter.BLL.DTOs.Admin.Localization.FeedbackHistories;

public record CreateFeedbackHistoryLocalizationDto
{
    public long EntityId { get; init; }
    public long LanguageId { get; init; }
    public string Title { get; init; } = null!;
    public string Story { get; init; } = null!;
}
