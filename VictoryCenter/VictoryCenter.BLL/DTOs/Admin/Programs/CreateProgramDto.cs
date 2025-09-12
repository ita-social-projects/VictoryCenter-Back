using VictoryCenter.DAL.Enums;
namespace VictoryCenter.BLL.DTOs.Admin.Programs;

public record CreateProgramDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; init; }
    public Status Status { get; init; }
    public long? ImageId { get; init; }
    public List<long> CategoryIds { get; init; } = [];
}
