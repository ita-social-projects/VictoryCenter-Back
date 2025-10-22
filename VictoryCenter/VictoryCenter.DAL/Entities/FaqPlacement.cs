using VictoryCenter.DAL.Entities.Interfaces;

namespace VictoryCenter.DAL.Entities;

public class FaqPlacement : IOrderableEntity
{
    public long PageId { get; set; }

    public VisitorPage Page { get; set; } = null!;

    public long QuestionId { get; set; }

    public FaqQuestion Question { get; set; } = null!;

    public long Priority { get; set; }
}
