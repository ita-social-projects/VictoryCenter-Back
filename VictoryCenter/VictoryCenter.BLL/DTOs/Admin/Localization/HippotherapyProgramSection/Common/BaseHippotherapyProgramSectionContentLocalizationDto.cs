namespace VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection.Common;
public record BaseHippotherapyProgramSectionContentLocalizationDto
{
    public string? Title { get; init; }

    public string? Description { get; init; }

    public string? Author { get; init; }

    public string? Question { get; init; }

    public string? Answer { get; init; }
}
