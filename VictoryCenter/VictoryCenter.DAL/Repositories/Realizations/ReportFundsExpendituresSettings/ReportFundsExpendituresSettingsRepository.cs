using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Repositories.Interfaces.ReportFundsExpendituresSettings;
using VictoryCenter.DAL.Repositories.Realizations.Base;
using ReportFundsExpendituresSettingsEntity = VictoryCenter.DAL.Entities.ReportFundsExpendituresSettings;

namespace VictoryCenter.DAL.Repositories.Realizations.ReportFundsExpendituresSettings;

public class ReportFundsExpendituresSettingsRepository
    : RepositoryBase<ReportFundsExpendituresSettingsEntity>, IReportFundsExpendituresSettingsRepository
{
    public ReportFundsExpendituresSettingsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
