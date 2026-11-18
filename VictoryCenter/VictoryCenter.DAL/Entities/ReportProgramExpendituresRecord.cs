using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities;

[Auditable]
public class ReportProgramExpendituresRecord : BaseEntity
{
    [Audited]
    public int ReportingYear { get; init; }

    [Audited]
    public long HippotherapyProgramCategoryId { get; set; }

    [Audited]
    public decimal AmountUah { get; set; }

    [Audited]
    public decimal AmountUsd { get; set; }

    public HippotherapyProgramCategory HippotherapyProgramCategory { get; set; } = null!;
}
