using System.Transactions;
using VictoryCenter.DAL.Repositories.Interfaces.Categories;
using VictoryCenter.DAL.Repositories.Interfaces.TeamMembers;
using VictoryCenter.DAL.Repositories.Interfaces.Test;
using VictoryCenter.DAL.Repositories.Interfaces.TeamMembers;

namespace VictoryCenter.DAL.Repositories.Interfaces.Base;

public interface IRepositoryWrapper
{
    ICategoriesRepository CategoriesRepository { get; }
    ITestRepository TestRepository { get; }
    ITeamMembersRepository TeamMembersRepository { get; }

    int SaveChanges();

    Task<int> SaveChangesAsync();

    TransactionScope BeginTransaction();
}
