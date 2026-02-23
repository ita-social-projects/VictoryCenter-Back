using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.ReportMediaSettings;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.ReportMediaSettings;

public class CollectedFundsBlockRepository : RepositoryBase<CollectedFundsBlock>, ICollectedFundsBlockRepository
{
    public CollectedFundsBlockRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
