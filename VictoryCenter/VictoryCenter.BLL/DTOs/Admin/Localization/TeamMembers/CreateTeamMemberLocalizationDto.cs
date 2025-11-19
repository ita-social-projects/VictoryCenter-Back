namespace VictoryCenter.BLL.DTOs.Admin.Localization.TeamMembers;

public record CreateTeamMemberLocalizationDto : UpdateTeamMemberLocalizationDto
{
    public long EntityId { get; init; }

    public long LanguageId { get; init; }
}
