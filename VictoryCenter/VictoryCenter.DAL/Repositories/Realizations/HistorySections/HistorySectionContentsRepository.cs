using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities.HistoryContents;
using VictoryCenter.DAL.Repositories.Interfaces.HistorySections;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.HistorySections;

public class HistorySectionContentsRepository : RepositoryBase<HistorySectionContent>, IHistorySectionContentsRepository
{
    public HistorySectionContentsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
