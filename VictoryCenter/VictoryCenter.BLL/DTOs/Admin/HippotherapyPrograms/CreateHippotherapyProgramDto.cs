using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;

public record CreateHippotherapyProgramDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public Status Status { get; set; }
    public string? Location { get; set; }
    public string? ParticipantsCount { get; set; }
    public string? MeetingsCount { get; set; }
    public long? BackgroundImageId { get; set; }
    public long? PreviewImageId { get; set; }
    public List<long> CategoryIds { get; set; } = [];
}
