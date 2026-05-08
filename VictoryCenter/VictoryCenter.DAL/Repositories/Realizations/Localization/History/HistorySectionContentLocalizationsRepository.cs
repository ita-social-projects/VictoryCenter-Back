using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.History;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Localization.History;

public class HistorySectionContentLocalizationsRepository : RepositoryBase<HistorySectionContentLocalization>, IHistorySectionContentLocalizationsRepository
{
    public HistorySectionContentLocalizationsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
