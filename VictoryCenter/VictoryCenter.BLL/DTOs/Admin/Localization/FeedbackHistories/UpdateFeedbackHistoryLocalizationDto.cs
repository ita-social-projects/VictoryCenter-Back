namespace VictoryCenter.BLL.DTOs.Admin.Localization.FeedbackHistories;

public record UpdateFeedbackHistoryLocalizationDto
{
    public string Title { get; init; } = null!;
    public string Story { get; init; } = null!;
}
