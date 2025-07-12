using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.TeamMembers;

public record CreateTeamMemberDto
{
    public string FullName { get; set; } = null!;

    public long CategoryId { get; set; }

    public Status Status { get; set; }

    public string? Description { get; set; }

    public string? Email { get; set; }

    public long? ImageId { get; set; }
}
