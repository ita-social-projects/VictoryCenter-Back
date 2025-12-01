namespace VictoryCenter.BLL.DTOs.Admin.TeamMembers;

public record SearchTeamMemberDto
{
    public string FullName { get; init; } = null!;

    public int? Offset { get; init; }

    public int? Limit { get; init; }
}
