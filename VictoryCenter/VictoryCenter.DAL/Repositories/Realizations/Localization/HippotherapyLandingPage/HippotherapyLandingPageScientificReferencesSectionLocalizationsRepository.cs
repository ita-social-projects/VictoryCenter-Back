using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.HippotherapyLandingPage;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Localization.HippotherapyLandingPage;

public class HippotherapyLandingPageScientificReferencesSectionLocalizationsRepository
    : RepositoryBase<HippotherapyLandingPageScientificReferencesSectionLocalization>,
      IHippotherapyLandingPageScientificReferencesSectionLocalizationsRepository
{
    public HippotherapyLandingPageScientificReferencesSectionLocalizationsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
