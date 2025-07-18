using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.DTOs.TeamMembers;

public record TeamMembersFilterDto : BaseFilterDto
{
    public long? CategoryId { get; set; }
}
