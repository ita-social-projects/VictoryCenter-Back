namespace VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgram;
public record BaseHippotherapyProgramLocalizationDto
{
    public string? Name { get; init; }

    public string? Description { get; init; }

    public string? Location { get; init; }

    public string? ParticipantsCount { get; init; }

    public string? MeetingsCount { get; init; }
}
