
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.DAL.Repositories.Interfaces
{
    public interface ITeamMemberRepository : IRepositoryBase<TeamMember>
    {
        Task<TeamMember?> GetByIdAsync(int id);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<TeamMember?> GetByEmailAsync(string email);
        Task<IEnumerable<TeamMember>> SearchAsync(string searchTerm);
        Task<int> GetCountAsync();
    }
}
