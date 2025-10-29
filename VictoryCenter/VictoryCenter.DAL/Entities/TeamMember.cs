using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

public class TeamMember : BaseEntity, IOrderableEntity
{
    public string FullName { get; set; } = null!;

    public long CategoryId { get; set; }

    public long Priority { get; set; }

    public Status Status { get; set; }

    public string? Description { get; set; }

    public long? ImageId { get; set; }

    public string? Email { get; set; }
    public TeamCategory TeamCategory { get; set; } = null!;

    public Image? Image { get; set; }
}
