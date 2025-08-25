using System.Transactions;
using VictoryCenter.DAL.Repositories.Interfaces.AboutUsContents;
using VictoryCenter.DAL.Repositories.Interfaces.Categories;
using VictoryCenter.DAL.Repositories.Interfaces.Media;
using VictoryCenter.DAL.Repositories.Interfaces.TeamMembers;

namespace VictoryCenter.DAL.Repositories.Interfaces.Base;

public interface IRepositoryWrapper
{
    ICategoriesRepository CategoriesRepository { get; }
    ITeamMembersRepository TeamMembersRepository { get; }
    IImageRepository ImageRepository { get; }

    IAboutUsContentsRepository AboutUsContentsRepository { get; }

    int SaveChanges();

    Task<int> SaveChangesAsync();

    TransactionScope BeginTransaction();
}
