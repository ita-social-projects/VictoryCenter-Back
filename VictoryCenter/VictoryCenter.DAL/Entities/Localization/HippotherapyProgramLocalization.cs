namespace VictoryCenter.DAL.Entities.Localization;

public class HippotherapyProgramLocalization : LocalizationBase<HippotherapyProgram>
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? ParticipantsCount { get; set; }
    public string? MeetingsCount { get; set; }
}
