using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.FeedbackHistories;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.FeedbackHistories;

public class FeedbackHistoriesRepository : RepositoryBase<FeedbackHistory>, IFeedbackHistoriesRepository
{
    public FeedbackHistoriesRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
