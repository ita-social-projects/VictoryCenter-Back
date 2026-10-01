using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.DTOs.Public.FeedbackHistories;

public record PublishedFeedbackHistoryDto
{
    public long Id { get; init; }
    public string Title { get; init; } = null!;
    public string Story { get; init; } = null!;
    public ImageDto? Image { get; init; }
}
