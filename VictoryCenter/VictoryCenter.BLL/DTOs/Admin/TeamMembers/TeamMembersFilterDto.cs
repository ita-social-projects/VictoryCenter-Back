using VictoryCenter.BLL.DTOs.Admin.Common;
using VictoryCenter.BLL.DTOs.Admin.Localization.Base;
using VictoryCenter.BLL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.TeamMembers;

public record TeamMembersFilterDto : BaseFilterDto, ITranslationStatusFilterDto
{
    public long? CategoryId { get; init; }
    public TranslationStatusFilter? TranslationStatusFilter { get; set; }
}
