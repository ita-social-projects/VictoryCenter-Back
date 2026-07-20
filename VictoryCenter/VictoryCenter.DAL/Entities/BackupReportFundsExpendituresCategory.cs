using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

public class BackupReportFundsExpendituresCategory : BaseEntity
{
    public string Name { get; set; } = "";
    public ReportFundsExpendituresType Type { get; set; }

    public ICollection<BackupReportFundsExpendituresCategoryLocalization> Localizations { get; set; } = [];
    public ICollection<BackupReportFundsExpendituresRecord> Records { get; set; } = [];
}
