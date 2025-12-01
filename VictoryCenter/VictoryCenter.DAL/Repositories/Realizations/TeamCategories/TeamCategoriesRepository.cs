using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.TeamCategories;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.TeamCategories;

public class TeamCategoriesRepository : RepositoryBase<TeamCategory>, ITeamCategoriesRepository
{
    public TeamCategoriesRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
