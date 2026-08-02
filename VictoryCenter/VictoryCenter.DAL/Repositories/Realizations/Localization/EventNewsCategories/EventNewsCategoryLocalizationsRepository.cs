using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.EventNewsCategories;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Localization.EventNewsCategories;

public class EventNewsCategoryLocalizationsRepository
    : RepositoryBase<EventNewsCategoryLocalization>, IEventNewsCategoryLocalizationsRepository
{
    public EventNewsCategoryLocalizationsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
