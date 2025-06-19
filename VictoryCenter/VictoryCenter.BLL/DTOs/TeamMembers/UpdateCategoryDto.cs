namespace VictoryCenter.BLL.DTOs.TeamMembers;

public record UpdateTeamMemberDto : CreateTeamMemberDto
{
    public long Id { get; set; }
    public int Order { get; set; }
}
