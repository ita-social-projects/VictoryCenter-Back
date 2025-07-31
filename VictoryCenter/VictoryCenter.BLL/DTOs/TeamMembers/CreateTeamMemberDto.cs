using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.TeamMembers;

public record CreateTeamMemberDto
{
    public string FullName { get; init; } = null!;

    public long CategoryId { get; init; }

    public Status Status { get; init; }

    public string? Description { get; init; }

    public long? ImageId { get; init; }

    public string? Email { get; init; }
}
