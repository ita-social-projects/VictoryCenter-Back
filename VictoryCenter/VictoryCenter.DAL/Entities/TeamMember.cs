using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

public class TeamMember
{
    public long Id { get; set; }

    public string FullName { get; set; } = null!;

    public long CategoryId { get; set; }

    public long Priority { get; set; }

    public Status Status { get; set; }

    public string? Description { get; set; }

#pragma warning disable SA1011
    public long? ImageId { get; set; }
#pragma warning restore SA1011

    public string? Email { get; set; }

    public DateTime CreatedAt { get; set; }

    public Category Category { get; set; } = default!;

    public Image? Image { get; set; }
}
