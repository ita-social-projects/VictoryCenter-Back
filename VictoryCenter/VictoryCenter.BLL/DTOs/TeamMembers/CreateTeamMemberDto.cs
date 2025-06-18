using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.TeamMembers;

public record CreateTeamMemberDto
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public long CategoryId { get; set; }

    public Status Status { get; set; }

    public string? Description { get; set; }

#pragma warning disable SA1011
    // Change private set to set when photo logic is added
    public byte[]? Photo { get; private set; }
#pragma warning restore SA1011

    public string? Email { get; set; }
}
