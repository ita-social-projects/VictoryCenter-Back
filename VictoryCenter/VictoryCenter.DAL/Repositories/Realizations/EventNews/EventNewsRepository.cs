using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
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
        long? categoryId,
        int offset,
        int limit,
        CancellationToken cancellationToken = default)
    {
        var priorityLinks = DbContext.Set<EventNewsEventNewsCategories>();

        return await DbContext.Set<EventNewsEntity>()
            .AsNoTracking()
            .Where(filter)
            .Select(eventNews => new
            {
                eventNews.Id,
                Priority = priorityLinks
                    .Where(link =>
                        link.EventsNewsId == eventNews.Id &&
                        (!categoryId.HasValue || link.CategoriesId == categoryId.Value))
                    .Select(link => (long?)link.Priority)
                    .Min() ?? long.MaxValue
            })
            .OrderBy(eventNews => eventNews.Priority)
            .ThenBy(eventNews => eventNews.Id)
            .Skip(offset)
            .Take(limit)
            .Select(eventNews => eventNews.Id)
            .ToListAsync(cancellationToken);
    }
}
