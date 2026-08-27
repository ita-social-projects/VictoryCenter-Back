using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.DTOs.Admin.FeedbackHistories;

public record FeedbackHistoryDto
{
    public long Id { get; init; }
    public string Title { get; init; } = null!;
    public string Story { get; init; } = null!;
    public ImageDto? Image { get; init; }
}
