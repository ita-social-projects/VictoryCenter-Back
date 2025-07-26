using VictoryCenter.BLL.DTOs.Categories;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.TeamMembers;

public record TeamMemberDto
{
    public long Id { get; set; }

    public string FullName { get; set; } = null!;

    public CategoryDto Category { get; set; }

    public long Priority { get; set; }

    public Status Status { get; set; }

    public string? Description { get; set; }

#pragma warning disable SA1011
    public byte[]? Photo { get; set; }
#pragma warning restore SA1011

    public string? Email { get; set; }
}
