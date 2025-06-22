using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.TeamMembers;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.TeamMembers;

public class TeamMembersRepository : RepositoryBase<TeamMember>, ITeamMembersRepository
{
    public TeamMembersRepository(VictoryCenterDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<List<TeamMember>> GetByCategoryIdAsync(long categoryId, bool orderByPriority = true)
    {
        var query = _dbContext.TeamMembers.Where(member => member.CategoryId == categoryId);

        if (orderByPriority)
        {
            query = query.OrderBy(member => member.Priority);
        }

        return await query.ToListAsync();
    }
}
