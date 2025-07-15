namespace VictoryCenter.BLL.DTOs.TeamMembers;

public record PublishedTeamMembersDto
{
    public long Id { get; init; }
    public string FullName { get; init; } = null!;
    public string? Description { get; init; }
}
