namespace VictoryCenter.DAL.Entities.HippotherapyProgramContents;

public class FaqQuestionProgramContent : ProgramSectionContent
{
    public long FaqQuestionId { get; set; }

    public FaqQuestion FaqQuestion { get; set; } = null!;
}
