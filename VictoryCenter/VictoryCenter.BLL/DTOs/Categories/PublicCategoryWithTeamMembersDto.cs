using VictoryCenter.BLL.DTOs.TeamMembers;

namespace VictoryCenter.BLL.DTOs.Categories;

public record PublicCategoryWithTeamMembersDto
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public List<PublicTeamMemberDto> Members { get; set; } = [];
}
