using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.BackupReportFundsExpenditures;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.BackupReportFundsExpenditures;

public class BackupReportProgramExpendituresRecordsRepository
    : RepositoryBase<BackupReportProgramExpendituresRecord>, IBackupReportProgramExpendituresRecordsRepository
{
    public BackupReportProgramExpendituresRecordsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
