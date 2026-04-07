using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.HistorySections;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.HistorySections;

public class HistorySectionsRepository : RepositoryBase<HistorySection>, IHistorySectionsRepository
{
    public HistorySectionsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
