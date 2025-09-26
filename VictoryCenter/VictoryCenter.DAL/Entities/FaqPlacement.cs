using VictoryCenter.DAL.Data.BaseEntity;

namespace VictoryCenter.DAL.Entities;

public class FaqPlacement : BaseEntity
{
    public long PageId { get; set; }

    public VisitorPage Page { get; set; } = null!;

    public long QuestionId { get; set; }

    public FaqQuestion Question { get; set; } = null!;

    public long Priority { get; set; }
}
