using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.EventNewsCategories;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.EventNewsCategories;

public class EventNewsCategoryRepository : RepositoryBase<EventNewsCategory>, IEventNewsCategoryRepository
{
    public EventNewsCategoryRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
