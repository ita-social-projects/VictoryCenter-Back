using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.TeamMembers;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.TeamMembers;

public class TeamMembersRepository : RepositoryBase<TeamMember>, ITeamMembersRepository
{
    public TeamMembersRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
