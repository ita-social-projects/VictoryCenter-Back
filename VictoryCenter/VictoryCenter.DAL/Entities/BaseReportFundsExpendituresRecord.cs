using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

public abstract class BaseReportFundsExpendituresRecord : BaseEntity
{
    public long CategoryId { get; set; }
    public ReportFundsExpendituresType Type { get; set; }
    public int ReportingYear { get; set; }
    public decimal AmountUah { get; set; }
    public decimal AmountUsd { get; set; }
}
