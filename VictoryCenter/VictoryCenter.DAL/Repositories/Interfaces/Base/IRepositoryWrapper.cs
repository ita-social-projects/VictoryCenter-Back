using System.Transactions;
using VictoryCenter.DAL.Repositories.Interfaces.Donate;
using VictoryCenter.DAL.Repositories.Interfaces.FaqPlacements;
using VictoryCenter.DAL.Repositories.Interfaces.FaqQuestions;
using VictoryCenter.DAL.Repositories.Interfaces.HippotherapyProgramCategories;
using VictoryCenter.DAL.Repositories.Interfaces.HippotherapyPrograms;
using VictoryCenter.DAL.Repositories.Interfaces.Media;
using VictoryCenter.DAL.Repositories.Interfaces.TeamCategories;
using VictoryCenter.DAL.Repositories.Interfaces.TeamMembers;
using VictoryCenter.DAL.Repositories.Interfaces.VisitorPages;
using VictoryCenter.DAL.Repositories.Interfaces.WhoWeAreContents;
using VictoryCenter.DAL.Repositories.Interfaces.WhoWeAreSections;

namespace VictoryCenter.DAL.Repositories.Interfaces.Base;

public interface IRepositoryWrapper
{
    ITeamCategoriesRepository TeamCategoriesRepository { get; }
    IFaqPlacementsRepository FaqPlacementsRepository { get; }
    IFaqQuestionsRepository FaqQuestionsRepository { get; }
    ITeamMembersRepository TeamMembersRepository { get; }
    IVisitorPagesRepository VisitorPagesRepository { get; }
    IImageRepository ImageRepository { get; }
    IHippotherapyProgramCategoriesRepository HippotherapyProgramCategoriesRepository { get; }
    IHippotherapyProgramsRepository HippotherapyProgramsRepository { get; }
    IUahBankDetailsRepository UahBankDetailsRepository { get; }
    IForeignBankDetailsRepository ForeignBankDetailsRepository { get; }
    ICorrespondentBankDetailsRepository CorrespondentBankDetailsRepository { get; }
    ISupportOptionsRepository SupportOptionsRepository { get; }
    IWhoWeAreContentsRepository WhoWeAreContentsRepository { get; }
    IWhoWeAreSectionsRepository WhoWeAreSectionsRepository { get; }

    IRepositoryBase<TEntity> GetRepository<TEntity>()
        where TEntity : class;

    int SaveChanges();

    Task<int> SaveChangesAsync();

    TransactionScope BeginTransaction();
}
