using VictoryCenter.BLL.DTOs.Admin.Localization.Base;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.TeamMembers;

public record CreateTeamMemberLocalizationDto : UpdateTeamMemberLocalizationDto, ILocalizationIdentity
{
    public long EntityId { get; init; }

    public long LanguageId { get; init; }
}
