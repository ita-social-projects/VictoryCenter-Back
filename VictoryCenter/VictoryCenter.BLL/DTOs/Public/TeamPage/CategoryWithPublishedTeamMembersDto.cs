namespace VictoryCenter.BLL.DTOs.Public.TeamPage;

public record CategoryWithPublishedTeamMembersDto
{
    public long Id { get; init; }
    public string CategoryName { get; init; } = null!;
    public string? Description { get; init; }
    public List<PublishedTeamMembersDto> TeamMembers { get; init; } = [];
}
