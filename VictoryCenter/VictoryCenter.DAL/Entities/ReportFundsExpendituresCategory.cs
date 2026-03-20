using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

public class ReportFundsExpendituresCategory : BaseEntity
{
    public string Name { get; set; } = "";

    public ReportFundsExpendituresType Type { get; set; }

    public ICollection<ReportFundsExpendituresRecord> Records { get; set; } = [];
}
