using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.BackupReportFundsExpenditures;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.BackupReportFundsExpenditures;

public class BackupReportFundsExpendituresSettingsRepository
    : RepositoryBase<BackupReportFundsExpendituresSettings>, IBackupReportFundsExpendituresSettingsRepository
{
    public BackupReportFundsExpendituresSettingsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
