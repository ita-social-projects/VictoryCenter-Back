using VictoryCenter.BLL.DTOs.Admin.Common;
using VictoryCenter.BLL.DTOs.Admin.Localization.Base;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.TeamMembers;

public record TeamMembersFilterDto : BaseFilterDto, ITranslationStatusFilterDto
{
    public long? CategoryId { get; init; }
    public TranslationStatus? TranslationStatus { get; set; }
}
