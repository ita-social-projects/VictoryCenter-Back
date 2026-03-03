using System.Transactions;
using VictoryCenter.DAL.Repositories.Interfaces.Donate;
using VictoryCenter.DAL.Repositories.Interfaces.FaqPlacements;
using VictoryCenter.DAL.Repositories.Interfaces.FaqQuestions;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.Languages;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.TeamMembers;
using VictoryCenter.DAL.Repositories.Interfaces.HippotherapyProgramCategories;
using VictoryCenter.DAL.Repositories.Interfaces.HippotherapyPrograms;
using VictoryCenter.DAL.Repositories.Interfaces.Media;
using VictoryCenter.DAL.Repositories.Interfaces.TeamCategories;
using VictoryCenter.DAL.Repositories.Interfaces.TeamMembers;
using VictoryCenter.DAL.Repositories.Interfaces.VisitorPages;
using VictoryCenter.DAL.Repositories.Interfaces.WhoWeAreContents;
using VictoryCenter.DAL.Repositories.Interfaces.WhoWeAreSections;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.FaqQuestions;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.HippotherapyPrograms;
using VictoryCenter.DAL.Repositories.Interfaces.Partners;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.TeamCategories;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.WhoWeAreContents;
using VictoryCenter.DAL.Repositories.Interfaces.ReportMediaSettings;

namespace VictoryCenter.DAL.Repositories.Interfaces.Base;

public interface IRepositoryWrapper
{
    ITeamCategoriesRepository TeamCategoriesRepository { get; }
    IFaqPlacementsRepository FaqPlacementsRepository { get; }
    IFaqQuestionsRepository FaqQuestionsRepository { get; }
    ITeamMembersRepository TeamMembersRepository { get; }
    IVisitorPagesRepository VisitorPagesRepository { get; }
    IImageRepository ImageRepository { get; }
    IPdfReportRepository PdfReportRepository { get; }
    IHippotherapyProgramCategoriesRepository HippotherapyProgramCategoriesRepository { get; }
    IHippotherapyProgramsRepository HippotherapyProgramsRepository { get; }
    ILocalizationLanguagesRepository LocalizationLanguagesRepository { get; }
    ITeamMemberLocalizationsRepository TeamMemberLocalizationsRepository { get; }
    IUahBankDetailsRepository UahBankDetailsRepository { get; }
    IForeignBankDetailsRepository ForeignBankDetailsRepository { get; }
    ICorrespondentBankDetailsRepository CorrespondentBankDetailsRepository { get; }
    ISupportOptionsRepository SupportOptionsRepository { get; }
    IWhoWeAreContentsRepository WhoWeAreContentsRepository { get; }
    IWhoWeAreContentLocalizationsRepository WhoWeAreContentLocalizationsRepository { get; }
    IWhoWeAreSectionsRepository WhoWeAreSectionsRepository { get; }
    IFaqQuestionLocalizationsRepository FaqQuestionLocalizationsRepository { get; }
    IPartnerRepository PartnerRepository { get; }
    IPartnerSectionsRepository PartnerSectionsRepository { get; }
    IPartnersPageBannersRepository PartnersPageBannersRepository { get; }
    IHippotherapyProgramsLocalizationsRepository HippotherapyProgramsLocalizationsRepository { get; }
    IProgramSectionContentsRepository ProgramSectionContentsRepository { get; }
    IProgramSectionContentLocalizationsRepository ProgramSectionContentLocalizationsRepository { get; }

    ITeamCategoryLocalizationsRepository TeamCategoryLocalizationsRepository { get; }

    IChangedLivesBlockRepository ChangedLivesBlockRepository { get; }

    ICollectedFundsBlockRepository CollectedFundsBlockRepository { get; }

    IRepositoryBase<TEntity> GetRepository<TEntity>()
        where TEntity : class;

    int SaveChanges();

    Task<int> SaveChangesAsync();

    TransactionScope BeginTransaction();
}
