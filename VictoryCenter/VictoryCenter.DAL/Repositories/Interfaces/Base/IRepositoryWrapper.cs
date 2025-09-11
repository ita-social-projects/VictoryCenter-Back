using System.Transactions;
using VictoryCenter.DAL.Repositories.Interfaces.Media;
using VictoryCenter.DAL.Repositories.Interfaces.ProgramCategories;
using VictoryCenter.DAL.Repositories.Interfaces.Programs;
using VictoryCenter.DAL.Repositories.Interfaces.TeamCategories;
using VictoryCenter.DAL.Repositories.Interfaces.TeamMembers;

namespace VictoryCenter.DAL.Repositories.Interfaces.Base;

public interface IRepositoryWrapper
{
    ITeamCategoriesRepository TeamCategoriesRepository { get; }
    ITeamMembersRepository TeamMembersRepository { get; }
    IImageRepository ImageRepository { get; }
    IProgramCategoriesRepository ProgramCategoriesRepository { get; }
    IProgramsRepository ProgramsRepository { get; }

    int SaveChanges();

    Task<int> SaveChangesAsync();

    TransactionScope BeginTransaction();
}
