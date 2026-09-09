using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.FeedbackHistories;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Localization.FeedbackHistories;

public class FeedbackHistoryLocalizationsRepository
    : RepositoryBase<FeedbackHistoryLocalization>, IFeedbackHistoryLocalizationsRepository
{
    public FeedbackHistoryLocalizationsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
