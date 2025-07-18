namespace VictoryCenter.DAL.Entities;

public class FaqPageQuestion
{
    public long PageId { get; set; }

    public Page Page { get; set; } = default!;

    public long QuestionId { get; set; }

    public FaqQuestion Question { get; set; } = default!;

    public long Priority { get; set; }
}
