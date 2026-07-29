using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.BackupReportFundsExpenditures;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.BackupReportFundsExpenditures;

public class BackupReportFundsExpendituresCategoryLocalizationsRepository
    : RepositoryBase<BackupReportFundsExpendituresCategoryLocalization>, IBackupReportFundsExpendituresCategoryLocalizationsRepository
{
    public BackupReportFundsExpendituresCategoryLocalizationsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
