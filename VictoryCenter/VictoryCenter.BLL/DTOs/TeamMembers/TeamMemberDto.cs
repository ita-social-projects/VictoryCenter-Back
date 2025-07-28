using VictoryCenter.BLL.DTOs.Categories;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.TeamMembers;

public record TeamMemberDto
{
    public long Id { get; init; }

    public string FullName { get; init; } = null!;

    public CategoryDto Category { get; init; }

    public long Priority { get; init; }

    public Status Status { get; init; }

    public string? Description { get; init; }

#pragma warning disable SA1011
    public byte[]? Photo { get; init; }
#pragma warning restore SA1011

    public string? Email { get; init; }
}
