using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Repositories.Interfaces.EventNews;
using VictoryCenter.DAL.Repositories.Realizations.Base;
using EventNewsEntity = VictoryCenter.DAL.Entities.EventNews;

namespace VictoryCenter.DAL.Repositories.Realizations.EventNews;

public class EventNewsRepository : RepositoryBase<EventNewsEntity>, IEventNewsRepository
{
    public EventNewsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }

    public async Task<IReadOnlyCollection<string>> GetSlugsStartingWithAsync(
        long excludedId,
        string slugPrefix,
        CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<EventNewsEntity>()
            .AsNoTracking()
            .Where(eventNews =>
                eventNews.Id != excludedId
                && eventNews.Slug != null
                && eventNews.Slug.StartsWith(slugPrefix))
            .Select(eventNews => eventNews.Slug!)
            .ToListAsync(cancellationToken);
    }
}
