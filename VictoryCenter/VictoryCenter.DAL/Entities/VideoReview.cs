using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

public class VideoReview : BaseEntity, IOrderableEntity
{
    public string Title { get; set; } = null!;

    public string Link { get; set; } = null!;

    public bool IsArchived { get; set; }

    public DateTimeOffset? ArchivedAt { get; set; }

    public long Priority { get; set; }

    public Status Status { get; set; }
}
