namespace VictoryCenter.BLL.DTOs.TeamMembers;

public class ReorderTeamMembersDto
{
    public long CategoryId { get; set; }
    public List<long> OrderedIds { get; set; } = [];
}
