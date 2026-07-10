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
}
