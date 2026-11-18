using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities.HippotherapyProgramContents;

public class DescriptionProgramContent : ProgramSectionContent
{
    [Audited]
    public string Description { get; set; } = null!;
}
