using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

public class TeamMember : IOrderableEntity
{
    public long Id { get; set; }

    public string FullName { get; set; } = null!;

    public long CategoryId { get; set; }

    public long Priority { get; set; }

    public Status Status { get; set; }

    public string? Description { get; set; }

    public long? ImageId { get; set; }

    public string? Email { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public Category Category { get; set; } = null!;

    public Image? Image { get; set; }

    public ICollection<TeamMemberLocalization> Localizations { get; set; } = [];
}
