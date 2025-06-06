using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VictoryCenter.DAL.Entities;

public class Category
{
    public long Id { get; set; }

    public string Name { get; set; } = default!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
}
