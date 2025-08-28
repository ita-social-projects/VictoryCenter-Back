namespace VictoryCenter.BLL.DTOs.TeamMembers;

public class ReorderTeamMembersDto
{
    public long CategoryId { get; init; }
    public List<long> OrderedIds { get; init; } = [];
}
