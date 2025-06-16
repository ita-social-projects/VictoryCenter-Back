using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.TeamMember;

public record TeamMemberDto
{
    public long Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string CategoryName { get; set; } = null!;

    public long Priority { get; set; }

    public Status Status { get; set; }

    public string? Description { get; set; }

    public byte[]? Photo { get; set; }

    public string? Email { get; set; }
}
