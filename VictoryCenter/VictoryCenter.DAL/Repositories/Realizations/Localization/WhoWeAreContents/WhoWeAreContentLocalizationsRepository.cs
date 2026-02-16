using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.WhoWeAreContents;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Localization.WhoWeAreContents;

public class WhoWeAreContentLocalizationsRepository : RepositoryBase<WhoWeAreContentLocalization>, IWhoWeAreContentLocalizationsRepository
{
    public WhoWeAreContentLocalizationsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
