namespace VictoryCenter.BLL.DTOs.Admin.TeamMembers;

public record ReorderTeamMembersDto
{
    public long TeamMemberId { get; init; }
    public long AfterTeamMemberId { get; init; }
    public long CategoryId { get; init; }

    public List<long> OrderedIds { get; init; } = [];
}
