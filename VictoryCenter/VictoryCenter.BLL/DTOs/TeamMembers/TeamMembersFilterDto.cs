using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.TeamMembers;

public record TeamMembersFilterDto
{
    public int Offset { get; set; }

    public int Limit { get; set; }

    public Status? Status { get; set; }

    public string? CategoryName { get; set; }
}
