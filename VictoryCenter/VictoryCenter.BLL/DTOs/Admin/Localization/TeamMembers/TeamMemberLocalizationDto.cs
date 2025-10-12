using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.TeamMembers;

public record TeamMemberLocalizationDto
{
    public long TeamMemberId { get; init; }

    public LocalizationLanguageDto LocalizationLanguageDto { get; init; } = null!;

    public string FullName { get; init; } = null!;

    public string? Description { get; init; }
}
