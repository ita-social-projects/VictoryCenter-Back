namespace VictoryCenter.BLL.DTOs.TeamMembers;

public record PublishedTeamMembersDto
{
    public long Id { get; set; }
    public string FullName { get; set; } = null!;
    public string? Description { get; set; }
}
