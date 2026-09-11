using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.FeedbackReviews;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Localization.FeedbackReviews;

public class FeedbackReviewLocalizationsRepository
    : RepositoryBase<FeedbackReviewLocalization>, IFeedbackReviewLocalizationsRepository
{
    public FeedbackReviewLocalizationsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
