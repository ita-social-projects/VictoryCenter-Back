using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.HippotherapyLandingPage;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Localization.HippotherapyLandingPage;

public class HippotherapyLandingPageAnalysisSectionLocalizationsRepository
    : RepositoryBase<HippotherapyLandingPageAnalysisSectionLocalization>,
      IHippotherapyLandingPageAnalysisSectionLocalizationsRepository
{
    public HippotherapyLandingPageAnalysisSectionLocalizationsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
