namespace VictoryCenter.BLL.DTOs.Admin.TeamMembers;

public record ReorderTeamMembersDto
{
    public long CategoryId { get; set; }
    public List<long> OrderedIds { get; set; } = [];
}
