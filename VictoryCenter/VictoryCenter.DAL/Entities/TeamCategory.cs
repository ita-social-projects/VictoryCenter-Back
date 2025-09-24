using VictoryCenter.DAL.Data.BaseEntity;

namespace VictoryCenter.DAL.Entities;

public class TeamCategory : BaseEntity
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public ICollection<TeamMember> TeamMembers { get; set; } = [];
}
