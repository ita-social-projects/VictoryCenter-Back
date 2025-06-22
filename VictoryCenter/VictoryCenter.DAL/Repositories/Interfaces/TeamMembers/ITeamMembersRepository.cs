using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.DAL.Repositories.Interfaces.TeamMembers;

public interface ITeamMembersRepository : IRepositoryBase<TeamMember>
{
    Task<List<TeamMember>> GetByCategoryIdAsync(long categoryId, bool orderByPriority = true);
}
