using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.TeamCategories;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Localization.TeamCategories;
public class TeamCategoryLocalizationRepository : RepositoryBase<TeamCategoryLocalization>, ITeamCategoryLocalizationsRepository
{
    public TeamCategoryLocalizationRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
