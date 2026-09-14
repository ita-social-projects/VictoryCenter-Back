using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.VideoReviews;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Localization.VideoReviews;

public class VideoReviewLocalizationsRepository
    : RepositoryBase<VideoReviewLocalization>, IVideoReviewLocalizationsRepository
{
    public VideoReviewLocalizationsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
