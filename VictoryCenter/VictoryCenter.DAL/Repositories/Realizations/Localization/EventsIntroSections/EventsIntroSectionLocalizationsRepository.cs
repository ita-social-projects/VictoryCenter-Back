using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.EventsIntroSections;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Localization.EventsIntroSections;

public class EventsIntroSectionLocalizationsRepository
    : RepositoryBase<EventsIntroSectionLocalization>, IEventsIntroSectionLocalizationsRepository
{
    public EventsIntroSectionLocalizationsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
