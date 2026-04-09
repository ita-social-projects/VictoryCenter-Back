using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.ReportProgramExpendituresCategories;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.ReportProgramExpendituresCategories;

public class ReportProgramExpendituresCategoriesRepository
    : RepositoryBase<ReportProgramExpendituresCategory>, IReportProgramExpendituresCategoriesRepository
{
    public ReportProgramExpendituresCategoriesRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
