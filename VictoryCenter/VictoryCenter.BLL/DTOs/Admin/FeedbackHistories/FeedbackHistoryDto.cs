using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.FeedbackHistories;

public record FeedbackHistoryDto
{
    public long Id { get; init; }
    public string Title { get; init; } = null!;
    public string Story { get; init; } = null!;
    public ImageDto? Image { get; init; }
    public long Priority { get; init; }
    public Status Status { get; init; }
}
