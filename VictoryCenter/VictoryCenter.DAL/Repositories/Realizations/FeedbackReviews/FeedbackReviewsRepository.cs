using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.FeedbackReviews;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.FeedbackReviews;

public class FeedbackReviewsRepository : RepositoryBase<FeedbackReview>, IFeedbackReviewsRepository
{
    public FeedbackReviewsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
