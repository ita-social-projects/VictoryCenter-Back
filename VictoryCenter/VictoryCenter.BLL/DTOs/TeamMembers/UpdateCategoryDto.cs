using VictoryCenter.BLL.DTOs.Categories;

namespace VictoryCenter.BLL.DTOs.TeamMembers;

public record UpdateTeamMemberDto : CreateTeamMemberDto
{
    public long Id { get; set; }
    public int Order { get; set; }

    public CategoryDto Category { get; set; } = null!;
}
