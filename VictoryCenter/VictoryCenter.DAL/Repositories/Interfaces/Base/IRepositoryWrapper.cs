using System.Transactions;
using VictoryCenter.DAL.Repositories.Interfaces.FaqPlacements;
using VictoryCenter.DAL.Repositories.Interfaces.FaqQuestions;
using VictoryCenter.DAL.Repositories.Interfaces.Media;
using VictoryCenter.DAL.Repositories.Interfaces.ProgramCategories;
using VictoryCenter.DAL.Repositories.Interfaces.Programs;
using VictoryCenter.DAL.Repositories.Interfaces.TeamCategories;
using VictoryCenter.DAL.Repositories.Interfaces.TeamMembers;
using VictoryCenter.DAL.Repositories.Interfaces.VisitorPages;

namespace VictoryCenter.DAL.Repositories.Interfaces.Base;

public interface IRepositoryWrapper
{
    ITeamCategoriesRepository TeamCategoriesRepository { get; }
    IFaqPlacementsRepository FaqPlacementsRepository { get; }
    IFaqQuestionsRepository FaqQuestionsRepository { get; }
    ITeamMembersRepository TeamMembersRepository { get; }
    IVisitorPagesRepository VisitorPagesRepository { get; }
    IImageRepository ImageRepository { get; }
    IProgramCategoriesRepository ProgramCategoriesRepository { get; }
    IProgramsRepository ProgramsRepository { get; }

    int SaveChanges();

    Task<int> SaveChangesAsync();

    TransactionScope BeginTransaction();
}
