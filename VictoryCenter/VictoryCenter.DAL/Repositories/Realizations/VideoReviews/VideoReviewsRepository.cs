using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.VideoReviews;
using VictoryCenter.DAL.Repositories.Realizations.Base;
using Microsoft.EntityFrameworkCore;

namespace VictoryCenter.DAL.Repositories.Realizations.VideoReviews;

public class VideoReviewsRepository : RepositoryBase<VideoReview>, IVideoReviewsRepository
{
    public VideoReviewsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }

    public Task<int> ArchiveAsync(long id, DateTimeOffset archivedAt)
    {
        return DbContext.Set<VideoReview>()
            .Where(videoReview => videoReview.Id == id && !videoReview.IsArchived)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(videoReview => videoReview.IsArchived, true)
                .SetProperty(videoReview => videoReview.ArchivedAt, archivedAt));
    }
}
