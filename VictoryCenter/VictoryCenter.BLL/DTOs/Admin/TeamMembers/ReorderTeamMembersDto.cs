namespace VictoryCenter.BLL.DTOs.Admin.TeamMembers;

public record ReorderTeamMembersDto
{
    public long CategoryId { get; init; }
    public List<long> OrderedIds { get; init; } = [];
}
