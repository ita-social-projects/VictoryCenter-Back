using VictoryCenter.DAL.Entities.HippotherapyProgramContents;

namespace VictoryCenter.DAL.Entities.Localization;

public class ProgramSectionContentLocalization : LocalizationBase<ProgramSectionContent>
{
    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Author { get; set; }

    public string? Question { get; set; }

    public string? Answer { get; set; }
}
