using System.Linq.Expressions;
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

    public async Task<IReadOnlyCollection<long>> GetPagedIdsByFilterAsync(
        Expression<Func<EventNewsEntity, bool>> filter,
        int offset,
        int limit,
        CancellationToken cancellationToken = default)
    {
        var query = DbContext.Set<EventNewsEntity>()
            .AsNoTracking()
            .Where(filter);

        return await query
            .OrderBy(eventNews => eventNews.Priority)
            .ThenBy(eventNews => eventNews.Id)
            .Skip(offset)
            .Take(limit)
            .Select(eventNews => eventNews.Id)
            .ToListAsync(cancellationToken);
    }
}
