using System.Transactions;
using VictoryCenter.DAL.Repositories.Interfaces.Categories;
using VictoryCenter.DAL.Repositories.Interfaces.FaqPlacements;
using VictoryCenter.DAL.Repositories.Interfaces.FaqQuestions;
using VictoryCenter.DAL.Repositories.Interfaces.TeamMembers;
using VictoryCenter.DAL.Repositories.Interfaces.VisitorPages;

namespace VictoryCenter.DAL.Repositories.Interfaces.Base;

public interface IRepositoryWrapper
{
    ICategoriesRepository CategoriesRepository { get; }
    IFaqPlacementsRepository FaqPlacementsRepository { get; }
    IFaqQuestionsRepository FaqQuestionsRepository { get; }
    ITeamMembersRepository TeamMembersRepository { get; }
    IVisitorPagesRepository VisitorPagesRepository { get; }

    int SaveChanges();

    Task<int> SaveChangesAsync();

    TransactionScope BeginTransaction();
}
