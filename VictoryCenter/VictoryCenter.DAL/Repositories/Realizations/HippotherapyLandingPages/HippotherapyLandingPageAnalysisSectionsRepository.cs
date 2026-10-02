using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.DAL.Repositories.Interfaces.HippotherapyLandingPages;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.HippotherapyLandingPages;

public class HippotherapyLandingPageAnalysisSectionsRepository
    : RepositoryBase<HippotherapyLandingPageAnalysisSection>, IHippotherapyLandingPageAnalysisSectionsRepository
{
    public HippotherapyLandingPageAnalysisSectionsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
