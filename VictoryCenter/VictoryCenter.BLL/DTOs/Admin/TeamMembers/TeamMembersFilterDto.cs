using VictoryCenter.BLL.DTOs.Admin.Common;

namespace VictoryCenter.BLL.DTOs.Admin.TeamMembers;

public record TeamMembersFilterDto : BaseFilterDto
{
    public long? CategoryId { get; set; }
}
