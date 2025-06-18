using VictoryCenter.BLL.DTOs.Categories;

namespace VictoryCenter.BLL.DTOs.TeamMembers;

public record UpdateTeamMemberDto : CreateTeamMemberDto
{
    public int Id { get; set; }
    public int Order { get; set; }

    public CategoryDto Category { get; set; } = null!;
}
