using VictoryCenter.BLL.DTOs.TeamMembers;

namespace VictoryCenter.BLL.DTOs.Categories;

public record CategoryWithPublishedTeamMembersDto
{
    public long Id { get; set; }
    public string CategoryName { get; set; } = null!;
    public string? Description { get; set; }
    public List<PublishedTeamMembersDto> TeamMembers { get; set; } = [];
}
