using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.VideoReviews;

public record VideoReviewDto
{
    public long Id { get; init; }

    public string Title { get; init; } = null!;

    public string Link { get; init; } = null!;

    public long Priority { get; init; }

    public Status Status { get; init; }
}
