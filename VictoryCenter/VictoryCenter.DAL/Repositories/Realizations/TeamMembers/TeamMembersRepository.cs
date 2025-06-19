using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.TeamMembers;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.TeamMembers;

public class TeamMembersRepository : RepositoryBase<TeamMember>, ITeamMembersRepository
{
    private readonly VictoryCenterDbContext _context;

    public TeamMembersRepository(VictoryCenterDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<TeamMember?> GetByEmailAsync(string email)
    {
        return await _context.TeamMembers
            .FirstOrDefaultAsync(tm => tm.Email != null && tm.Email.ToLower() == email.ToLower());
    }

    public async Task<IEnumerable<TeamMember>> SearchAsync(string searchTerm)
    {
        return await _context.TeamMembers
            .Where(tm => tm.FirstName.Contains(searchTerm) || (tm.Email != null && tm.Email.Contains(searchTerm)))
            .ToListAsync();
    }

    public async Task<int> GetCountAsync()
    {
        return await _context.TeamMembers.CountAsync();
    }
}
