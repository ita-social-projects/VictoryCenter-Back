using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.EventsIntroSections;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.EventsIntroSections;

public class EventsIntroSectionsRepository : RepositoryBase<EventsIntroSection>, IEventsIntroSectionsRepository
{
    public EventsIntroSectionsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
