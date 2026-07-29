using VictoryCenter.DAL.Repositories.Interfaces.Base;
using EventNewsEntity = VictoryCenter.DAL.Entities.EventNews;

namespace VictoryCenter.DAL.Repositories.Interfaces.EventNews;

public interface IEventNewsRepository : IRepositoryBase<EventNewsEntity>
{
    Task<IReadOnlyCollection<string>> GetSlugsStartingWithAsync(
        long excludedId,
        string slugPrefix,
        CancellationToken cancellationToken = default);
}
