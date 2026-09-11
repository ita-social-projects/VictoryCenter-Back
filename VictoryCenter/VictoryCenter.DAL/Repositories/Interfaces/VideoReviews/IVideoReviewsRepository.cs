using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.DAL.Repositories.Interfaces.VideoReviews;

public interface IVideoReviewsRepository : IRepositoryBase<VideoReview>
{
	Task<int> ArchiveAsync(long id, DateTimeOffset archivedAt);
}
