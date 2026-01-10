using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

public class HippotherapyProgram : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Description { get; set; }
    public Status Status { get; set; }
    public string? Location { get; set; }
    public string? ParticipantsCount { get; set; }
    public string? MeetingsCount { get; set; }
    public long? BackgroundImageId { get; set; }
    public Image? BackgroundImage { get; set; }
    public long? PreviewImageId { get; set; }
    public Image? PreviewImage { get; set; }
    public ICollection<HippotherapyProgramCategory> Categories { get; set; } = [];
    public ICollection<HippotherapyProgramSection> Sections { get; set; } = [];
}
