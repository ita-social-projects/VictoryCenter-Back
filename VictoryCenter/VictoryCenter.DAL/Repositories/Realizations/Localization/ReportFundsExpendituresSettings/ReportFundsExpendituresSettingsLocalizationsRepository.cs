using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.ReportFundsExpendituresSettings;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Localization.ReportFundsExpendituresSettings;

public class ReportFundsExpendituresSettingsLocalizationsRepository
    : RepositoryBase<ReportFundsExpendituresSettingsLocalization>,
      IReportFundsExpendituresSettingsLocalizationsRepository
{
    public ReportFundsExpendituresSettingsLocalizationsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
