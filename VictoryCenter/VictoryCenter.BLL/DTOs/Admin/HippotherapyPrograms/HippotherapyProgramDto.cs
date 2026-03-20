using VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramSection;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgram;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;

public record HippotherapyProgramDto
{
    public long Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Slug { get; init; }
    public string? Description { get; init; }
    public Status Status { get; init; }
    public string? Location { get; init; }
    public string? ParticipantsCount { get; init; }
    public string? MeetingsCount { get; init; }
    public ImageDto? BackgroundImage { get; init; }
    public ImageDto? PreviewImage { get; init; }
    public List<ProgramCategoryShortDto> Categories { get; init; } = [];
    public List<HippotherapyProgramSectionDto> Sections { get; init; } = [];
    public IEnumerable<HippotherapyProgramLocalizationDto> Localizations { get; init; } = [];
}
