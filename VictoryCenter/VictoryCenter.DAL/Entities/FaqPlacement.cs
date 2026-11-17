using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.Interfaces;

namespace VictoryCenter.DAL.Entities;

[Auditable]
public class FaqPlacement : IOrderableEntity
{
    [Audited]
    public long PageId { get; set; }

    public VisitorPage Page { get; set; } = null!;

    [Audited]
    public long QuestionId { get; set; }

    public FaqQuestion Question { get; set; } = null!;

    [Audited]
    public long Priority { get; set; }
}
