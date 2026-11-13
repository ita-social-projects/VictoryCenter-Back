using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities.HippotherapyProgramContents;

public class AuthorProgramContent : ProgramSectionContent
{
    [Audited]
    public string Name { get; set; } = null!;
}
