using System.Transactions;
using VictoryCenter.DAL.Repositories.Interfaces.TeamMembers;
using VictoryCenter.DAL.Repositories.Interfaces.Test;

namespace VictoryCenter.DAL.Repositories.Interfaces.Base;

public interface IRepositoryWrapper
{
    public ITestRepository TestRepository { get; }

    public ITeamMembersRepository TeamMembersRepository { get; }
    
    public int SaveChanges();

    public Task<int> SaveChangesAsync();

    public TransactionScope BeginTransaction();
}
