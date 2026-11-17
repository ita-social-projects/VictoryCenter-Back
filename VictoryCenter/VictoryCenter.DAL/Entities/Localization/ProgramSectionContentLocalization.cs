using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.HippotherapyProgramContents;

namespace VictoryCenter.DAL.Entities.Localization;

public class ProgramSectionContentLocalization : LocalizationBase<ProgramSectionContent>
{
    [Audited]
    public string? Title { get; set; }

    [Audited]
    public string? Description { get; set; }

    [Audited]
    public string? Author { get; set; }

    [Audited]
    public string? Question { get; set; }

    [Audited]
    public string? Answer { get; set; }
}
