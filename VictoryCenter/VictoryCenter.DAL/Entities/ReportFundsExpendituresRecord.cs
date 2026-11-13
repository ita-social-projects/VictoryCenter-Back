using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities;

[Auditable]
public class ReportFundsExpendituresRecord : BaseReportFundsExpendituresRecord
{
    public ReportFundsExpendituresCategory Category { get; set; } = null!;
}
