using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.TeamMembers;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Localization.TeamMembers;

public class TeamMemberLocalizationsRepository : RepositoryBase<TeamMemberLocalization>, ITeamMemberLocalizationsRepository
{
    public TeamMemberLocalizationsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
