using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.DAL.Repositories.Interfaces.TeamMembers;

public interface ITeamMembersRepository : IRepositoryBase<TeamMember>
{
    Task<TeamMember?> GetByEmailAsync(string email);
    Task<IEnumerable<TeamMember>> SearchAsync(string searchTerm);
    Task<int> GetCountAsync();
}
