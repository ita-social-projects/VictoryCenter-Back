using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.EventNews;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.EventNews;

public class EventNewsEventCategoriesRepository
    : RepositoryBase<EventNewsEventNewsCategories>,
    IEventNewsEventNewsCategoriesRepository
{
    public EventNewsEventCategoriesRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
