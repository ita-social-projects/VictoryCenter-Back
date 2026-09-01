using VictoryCenter.DAL.Data.BaseEntity;

namespace VictoryCenter.DAL.Entities;

public class VideoReview : BaseEntity
{
    public string Title { get; set; } = null!;

    public string Link { get; set; } = null!;
}
