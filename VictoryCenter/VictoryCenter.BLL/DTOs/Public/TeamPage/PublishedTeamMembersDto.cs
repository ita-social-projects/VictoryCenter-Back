using VictoryCenter.BLL.DTOs.Admin.Images;

namespace VictoryCenter.BLL.DTOs.Public.TeamPage;

public record PublishedTeamMembersDto
{
    public long Id { get; init; }
    public string FullName { get; init; } = null!;
    public string? Description { get; init; }
    public ImageDto? Image { get; init; }
}
