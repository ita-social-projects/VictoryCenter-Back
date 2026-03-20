using VictoryCenter.DAL.Data.BaseEntity;

namespace VictoryCenter.DAL.Entities;

public class ReportFundsExpendituresSettings : BaseEntity
{
    public string DisclaimerTitle { get; set; } = "";
    public decimal ExchangeRate { get; set; }
}
