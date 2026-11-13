using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities.Localization;

public class HippotherapyProgramLocalization : LocalizationBase<HippotherapyProgram>
{
    [Audited]
    public string Name { get; set; } = null!;

    [Audited]
    public string? Description { get; set; }

    [Audited]
    public string? Location { get; set; }

    [Audited]
    public string? ParticipantsCount { get; set; }

    [Audited]
    public string? MeetingsCount { get; set; }
}
