namespace VictoryCenter.BLL.DTOs.Admin.VideoReviews;

public record CreateVideoReviewDto
{
    public string Title { get; init; } = null!;

    public string Link { get; init; } = null!;
}
