namespace VictoryCenter.DAL.Entities;

public class Category
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public ICollection<TeamMember> TeamMembers { get; set; } = [];
}
