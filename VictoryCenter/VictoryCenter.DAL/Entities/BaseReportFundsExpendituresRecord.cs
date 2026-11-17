using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

public abstract class BaseReportFundsExpendituresRecord : BaseEntity
{
    [Audited]
    public long CategoryId { get; set; }

    [Audited]
    public ReportFundsExpendituresType Type { get; set; }

    [Audited]
    public int ReportingYear { get; set; }

    [Audited]
    public decimal AmountUah { get; set; }

    [Audited]
    public decimal AmountUsd { get; set; }
}
