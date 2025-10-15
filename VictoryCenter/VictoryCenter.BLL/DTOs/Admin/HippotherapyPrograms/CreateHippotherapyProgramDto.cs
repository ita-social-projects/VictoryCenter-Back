using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;

public record CreateHippotherapyProgramDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; init; }
    public Status Status { get; init; }
    public long? ImageId { get; init; }
    public List<long> CategoryIds { get; init; } = [];
}
