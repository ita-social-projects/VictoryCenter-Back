using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.MainPage;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.MainPage;

public class MetricRepository : RepositoryBase<Metric>, IMetricRepository
{
    public MetricRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
