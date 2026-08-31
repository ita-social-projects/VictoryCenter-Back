using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.VideoReviews;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.VideoReviews;

public class VideoReviewsRepository : RepositoryBase<VideoReview>, IVideoReviewsRepository
{
    public VideoReviewsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
