using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.TeamMembers;

public record CreateTeamMemberDto
{
    private string _fullName;

    public string FullName
    {
        init => _fullName = value == null ? string.Empty : value.Trim();
        get => _fullName;
    }

    public long CategoryId { get; init; }

    public Status Status { get; init; }

    public string? Description { get; init; }

    public long? ImageId { get; init; }

    public string? Email { get; init; }
}
