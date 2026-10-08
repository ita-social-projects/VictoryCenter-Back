using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.EventNews;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Localization.EventNews;

public class EventNewsLocalizationsRepository : RepositoryBase<EventNewsLocalization>, IEventNewsLocalizationsRepository
{
    public EventNewsLocalizationsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
