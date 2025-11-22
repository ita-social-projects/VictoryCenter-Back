using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.TeamMembers;

public record TeamMemberLocalizationDto
{
    public long EntityId { get; init; }

    public LocalizationInfoDto LocalizatioInfoDto { get; init; } = null!;

    public string FullName { get; init; } = null!;

    public string? Description { get; init; }

    public TranslationStatus TranslationStatus { get; init; }
}
