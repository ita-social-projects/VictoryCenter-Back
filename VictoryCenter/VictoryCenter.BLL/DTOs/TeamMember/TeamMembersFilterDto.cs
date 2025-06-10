using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.TeamMember;

public class TeamMembersFilterDto
{
    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public Status? Status { get; set; }

    public string? CategoryName { get; set; }
}
