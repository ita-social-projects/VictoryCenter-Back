using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities.HippotherapyProgramContents;

public class FaqQuestionProgramContent : ProgramSectionContent
{
    [Audited]
    public long FaqQuestionId { get; set; }

    public FaqQuestion FaqQuestion { get; set; } = null!;
}
