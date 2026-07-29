using VictoryCenter.DAL.Data.BaseEntity;

namespace VictoryCenter.DAL.Entities;

public class BackupReportFundsExpendituresSettings : BaseEntity
{
    public string DisclaimerTitle { get; set; } = "";
    public decimal ExchangeRate { get; set; }
    public int ProgramExpendituresReportingYear { get; set; }
}
