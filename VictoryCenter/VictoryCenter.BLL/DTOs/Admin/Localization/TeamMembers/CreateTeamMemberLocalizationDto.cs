namespace VictoryCenter.BLL.DTOs.Admin.Localization.TeamMembers;

public record CreateTeamMemberLocalizationDto
{
    public long TeamMemberId { get; init; }

    public long LanguageId { get; init; }

    public string FullName { get; init; } = null!;

    public string? Description { get; init; }
}
