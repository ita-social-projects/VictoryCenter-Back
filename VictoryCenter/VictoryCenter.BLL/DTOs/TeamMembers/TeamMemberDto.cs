using VictoryCenter.BLL.DTOs.Images;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.TeamMembers;

public record TeamMemberDto
{
    public long Id { get; set; }

    public string FullName { get; set; } = null!;

    public long CategoryId { get; set; }

    public long Priority { get; set; }

    public Status Status { get; set; }

    public string? Description { get; set; }

    public string? Email { get; set; }

    public ImageDTO? Image { get; set; }
}
