using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.ReportFundsExpendituresCategories;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.ReportFundsExpendituresCategories;

public class ReportFundsExpendituresCategoriesRepository : RepositoryBase<ReportFundsExpendituresCategory>, IReportFundsExpendituresCategoriesRepository
{
    public ReportFundsExpendituresCategoriesRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
