using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.HippotherapyLandingPage;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Localization.HippotherapyLandingPage;

public class HippotherapyLandingPageHippoventionSectionLocalizationsRepository
    : RepositoryBase<HippotherapyLandingPageHippoventionSectionLocalization>,
      IHippotherapyLandingPageHippoventionSectionLocalizationsRepository
{
    public HippotherapyLandingPageHippoventionSectionLocalizationsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
