using VictoryCenter.DAL.Enums;
using VictoryCenter.BLL.DTOs.Categories;
namespace VictoryCenter.BLL.DTOs.TeamMembers;

public record UpdateTeamMemberDto : CreateTeamMemberDto
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string? Description { get; set; }

    public Status Status { get; set; }

    public byte[]? Photo { get; set; }

    public int Order { get; set; }

    public CategoryDto Category { get; set; } = null!;
}
