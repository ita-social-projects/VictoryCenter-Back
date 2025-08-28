using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.TeamMembers;

public record TeamMembersFilterDto
{
    public int? Offset { get; init; }

    public int? Limit { get; init; }

    public Status? Status { get; init; }

    public long? CategoryId { get; init; }
}
