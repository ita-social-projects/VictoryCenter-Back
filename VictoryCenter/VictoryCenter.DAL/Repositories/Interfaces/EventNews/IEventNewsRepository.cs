using System.Linq.Expressions;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using EventNewsEntity = VictoryCenter.DAL.Entities.EventNews;

namespace VictoryCenter.DAL.Repositories.Interfaces.EventNews;

public interface IEventNewsRepository : IRepositoryBase<EventNewsEntity>
{
    Task<IReadOnlyCollection<string>> GetSlugsStartingWithAsync(
        long excludedId,
        string slugPrefix,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<long>> GetPagedIdsByFilterAsync(
        Expression<Func<EventNewsEntity, bool>> filter,
        int offset,
        int limit,
        CancellationToken cancellationToken = default);
}
