using VictoryCenter.BLL.DTOs.Images;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.TeamMembers;

public record TeamMemberDto
{
    public long Id { get; set; }

    public string FullName { get; set; } = null!;

    public string CategoryName { get; set; } = null!;

    public long Priority { get; set; }

    public Status Status { get; set; }

    public string? Description { get; set; }

#pragma warning disable SA1011
    public ImageDTO? Image { get; set; }
#pragma warning restore SA1011

    public string? Email { get; set; }
}
