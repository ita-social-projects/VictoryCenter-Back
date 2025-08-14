using VictoryCenter.BLL.DTOs.Admin.Images;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.TeamMembers;

public record TeamMemberDto
{
    public long Id { get; init; }

    public string FullName { get; init; } = null!;

    public long CategoryId { get; init; }

    public long Priority { get; init; }

    public Status Status { get; init; }

    public string? Description { get; init; }

    public string? Email { get; init; }

    public ImageDto? Image { get; set; }
}
