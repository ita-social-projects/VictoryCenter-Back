using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.VideoReviews;
using VictoryCenter.DAL.Repositories.Realizations.Base;
using Microsoft.EntityFrameworkCore;

namespace VictoryCenter.DAL.Repositories.Realizations.VideoReviews;

public class VideoReviewsRepository : RepositoryBase<VideoReview>, IVideoReviewsRepository
{
    private static readonly SemaphoreSlim InMemoryArchiveLock = new(1, 1);

    public VideoReviewsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }

    public async Task<int> ArchiveAsync(long id, DateTimeOffset archivedAt)
    {
        if (DbContext.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
        {
            return await DbContext.Set<VideoReview>()
                .Where(videoReview => videoReview.Id == id && !videoReview.IsArchived)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(videoReview => videoReview.IsArchived, true)
                    .SetProperty(videoReview => videoReview.ArchivedAt, archivedAt));
        }

        await InMemoryArchiveLock.WaitAsync();
        try
        {
            var entity = await DbContext.Set<VideoReview>()
                .FirstOrDefaultAsync(videoReview => videoReview.Id == id && !videoReview.IsArchived);

            if (entity is null)
            {
                return 0;
            }

            entity.IsArchived = true;
            entity.ArchivedAt = archivedAt;

            return await DbContext.SaveChangesAsync();
        }
        finally
        {
            InMemoryArchiveLock.Release();
        }
    }
}
