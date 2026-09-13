namespace VictoryCenter.BLL.DTOs.Admin.VideoReviews;

public record ReorderVideoReviewsDto
{
    public List<long> OrderedIds { get; init; } = [];
}
