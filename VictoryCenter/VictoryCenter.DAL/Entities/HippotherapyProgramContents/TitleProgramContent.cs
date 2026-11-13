using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities.HippotherapyProgramContents;

public class TitleProgramContent : ProgramSectionContent
{
    [Audited]
    public string Title { get; set; } = null!;
}
