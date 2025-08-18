namespace VictoryCenter.DAL.Entities;

public class FaqPlacement
{
    public long PageId { get; set; }

    public VisitorPage Page { get; set; } = default!;

    public long QuestionId { get; set; }

    public FaqQuestion Question { get; set; } = default!;

    public long Priority { get; set; }
}
