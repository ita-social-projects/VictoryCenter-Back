namespace VictoryCenter.BLL.DTOs.Admin.Localization.TeamMembers;

public record UpdateTeamMemberLocalizationDto
{
    public string FullName { get; init; } = null!;

    public string? Description { get; init; }
}
