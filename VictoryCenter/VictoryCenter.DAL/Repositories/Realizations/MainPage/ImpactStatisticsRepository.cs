using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.MainPage;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.MainPage;

public class ImpactStatisticsRepository : RepositoryBase<ImpactStatistics>, IImpactStatisticsRepository
{
    public ImpactStatisticsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
