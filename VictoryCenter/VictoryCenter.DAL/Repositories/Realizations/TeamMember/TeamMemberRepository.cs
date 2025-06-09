using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces;
using VictoryCenter.DAL.Repositories.Realizations.Base;
using VictoryCenter.DAL.Data;


namespace VictoryCenter.DAL.Repositories.Realizations
{
    public class TeamMemberRepository : RepositoryBase<TeamMember>, ITeamMemberRepository
    {
        private readonly VictoryCenterDbContext _context;

        public TeamMemberRepository(VictoryCenterDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<TeamMember?> GetByIdAsync(int id)
        {
            return await _context.TeamMembers
                .FirstOrDefaultAsync(tm => tm.Id == id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var teamMember = await GetByIdAsync(id);
            if (teamMember == null)
                return false;

            _context.TeamMembers.Remove(teamMember);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.TeamMembers.AnyAsync(tm => tm.Id == id);
        }

        public async Task<TeamMember?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            return await _context.TeamMembers
                .FirstOrDefaultAsync(tm => tm.Email != null && tm.Email.ToLower() == email.ToLower());
        }


        public async Task<IEnumerable<TeamMember>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await _context.TeamMembers.ToListAsync();

            var term = searchTerm.ToLower();

            return await _context.TeamMembers
                .Where(tm =>
                    (!string.IsNullOrEmpty(tm.FirstName) && tm.FirstName.ToLower().Contains(term)) ||
                    (!string.IsNullOrEmpty(tm.LastName) && tm.LastName.ToLower().Contains(term)) ||
                    (!string.IsNullOrEmpty(tm.Email) && tm.Email.ToLower().Contains(term)))
                .ToListAsync();
        }


        public async Task<int> GetCountAsync()
        {
            return await _context.TeamMembers.CountAsync();
        }
    }

}
