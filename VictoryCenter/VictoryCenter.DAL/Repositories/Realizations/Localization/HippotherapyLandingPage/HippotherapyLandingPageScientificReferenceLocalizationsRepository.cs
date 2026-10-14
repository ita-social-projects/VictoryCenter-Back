using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.HippotherapyLandingPage;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Localization.HippotherapyLandingPage;

public class HippotherapyLandingPageScientificReferenceLocalizationsRepository
    : RepositoryBase<HippotherapyLandingPageScientificReferenceLocalization>,
      IHippotherapyLandingPageScientificReferenceLocalizationsRepository
{
    public HippotherapyLandingPageScientificReferenceLocalizationsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
