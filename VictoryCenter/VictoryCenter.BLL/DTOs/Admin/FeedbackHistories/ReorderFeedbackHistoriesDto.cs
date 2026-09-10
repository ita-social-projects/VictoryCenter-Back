namespace VictoryCenter.BLL.DTOs.Admin.FeedbackHistories;

public record ReorderFeedbackHistoriesDto
{
    public List<long> OrderedIds { get; init; } = [];
}
