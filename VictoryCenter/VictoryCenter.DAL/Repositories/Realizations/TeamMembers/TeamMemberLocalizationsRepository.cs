using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.TeamMembers;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.TeamMembers;

public class TeamMemberLocalizationsRepository : RepositoryBase<TeamMemberLocalization>, ITeamMemberLocalizationsRepository
{
    public TeamMemberLocalizationsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
