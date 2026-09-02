using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.FeedbackHistories;

public record CreateFeedbackHistoryDto
{
    public string Title { get; init; } = null!;
    public string Story { get; init; } = null!;
    public long? ImageId { get; init; }
    public Status Status { get; init; }
}
