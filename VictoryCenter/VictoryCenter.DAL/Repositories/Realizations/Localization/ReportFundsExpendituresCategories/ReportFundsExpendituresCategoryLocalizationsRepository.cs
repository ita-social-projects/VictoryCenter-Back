using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.ReportFundsExpendituresCategories;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Localization.ReportFundsExpendituresCategories;

public class ReportFundsExpendituresCategoryLocalizationsRepository
    : RepositoryBase<ReportFundsExpendituresCategoryLocalization>,
      IReportFundsExpendituresCategoryLocalizationsRepository
{
    public ReportFundsExpendituresCategoryLocalizationsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
