using System.Transactions;
using VictoryCenter.DAL.Repositories.Interfaces.CompanyProfile;
using VictoryCenter.DAL.Repositories.Interfaces.Donate;
using VictoryCenter.DAL.Repositories.Interfaces.FaqPlacements;
using VictoryCenter.DAL.Repositories.Interfaces.FaqQuestions;
using VictoryCenter.DAL.Repositories.Interfaces.HippotherapyProgramCategories;
using VictoryCenter.DAL.Repositories.Interfaces.HippotherapyPrograms;
using VictoryCenter.DAL.Repositories.Interfaces.HistorySections;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.CompanyProfile;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.FaqQuestions;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.HippotherapyPrograms;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.Languages;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.PdfSection;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.ReportFundsExpendituresCategories;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.ReportFundsExpendituresSettings;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.TeamCategories;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.TeamMembers;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.MainPage;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.WhoWeAreContents;
using VictoryCenter.DAL.Repositories.Interfaces.MainPage;
using VictoryCenter.DAL.Repositories.Interfaces.Media;
using VictoryCenter.DAL.Repositories.Interfaces.Partners;
using VictoryCenter.DAL.Repositories.Interfaces.PdfSection;
using VictoryCenter.DAL.Repositories.Interfaces.ReportFundsExpendituresCategories;
using VictoryCenter.DAL.Repositories.Interfaces.ReportFundsExpendituresRecords;
using VictoryCenter.DAL.Repositories.Interfaces.ReportFundsExpendituresSettings;
using VictoryCenter.DAL.Repositories.Interfaces.ReportMediaSettings;
using VictoryCenter.DAL.Repositories.Interfaces.ReportProgramExpendituresRecords;
using VictoryCenter.DAL.Repositories.Interfaces.TeamCategories;
using VictoryCenter.DAL.Repositories.Interfaces.TeamMembers;
using VictoryCenter.DAL.Repositories.Interfaces.VisitorPages;
using VictoryCenter.DAL.Repositories.Interfaces.WhoWeAreContents;
using VictoryCenter.DAL.Repositories.Interfaces.WhoWeAreSections;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.History;

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
    IPdfSectionRepository PdfSectionRepository { get; }
    IHippotherapyProgramCategoriesRepository HippotherapyProgramCategoriesRepository { get; }
    IHippotherapyProgramsRepository HippotherapyProgramsRepository { get; }
    ILocalizationLanguagesRepository LocalizationLanguagesRepository { get; }
    IPdfSectionLocalizationsRepository PdfSectionLocalizationsRepository { get; }
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

    IReportFundsExpendituresCategoryLocalizationsRepository ReportFundsExpendituresCategoryLocalizationsRepository { get; }

    IReportFundsExpendituresSettingsLocalizationsRepository ReportFundsExpendituresSettingsLocalizationsRepository { get; }

    IChangedLivesBlockRepository ChangedLivesBlockRepository { get; }

    ICollectedFundsBlockRepository CollectedFundsBlockRepository { get; }

    IReportFundsExpendituresCategoriesRepository ReportFundsExpendituresCategoriesRepository { get; }
    IReportFundsExpendituresRecordsRepository ReportFundsExpendituresRecordsRepository { get; }
    IReportFundsExpendituresSettingsRepository ReportFundsExpendituresSettingsRepository { get; }

    IReportProgramExpendituresRecordsRepository ReportProgramExpendituresRecordsRepository { get; }

    ICompanyProfileRepository CompanyProfileRepository { get; }
    ICompanyProfileContactRepository CompanyProfileContactRepository { get; }
    ICompanyProfileRequisiteRepository CompanyProfileRequisiteRepository { get; }
    ICompanyProfileContactLocalizationsRepository CompanyProfileContactLocalizationsRepository { get; }
    ICompanyProfileRequisiteLocalizationsRepository CompanyProfileRequisiteLocalizationsRepository { get; }

    IMainPageRepository MainPageRepository { get; }
    IMainAboutUsRepository MainAboutUsRepository { get; }
    IMainPartnersRepository MainPartnersRepository { get; }
    IMainDonationsRepository MainDonationsRepository { get; }
    IMainPageLocalizationsRepository MainPageLocalizationsRepository { get; }
    IMainAboutUsLocalizationsRepository MainAboutUsLocalizationsRepository { get; }
    IMainPartnersLocalizationsRepository MainPartnersLocalizationsRepository { get; }
    IMainDonationsLocalizationsRepository MainDonationsLocalizationsRepository { get; }
    IImpactStatisticsRepository ImpactStatisticsRepository { get; }
    IImpactStatisticsLocalizationsRepository ImpactStatisticsLocalizationsRepository { get; }
    IMetricRepository MetricRepository { get; }
    IMetricLocalizationsRepository MetricLocalizationsRepository { get; }
    IHistorySectionsRepository HistorySectionsRepository { get; }
    IHistorySectionContentsRepository HistorySectionContentsRepository { get; }
    IHistorySectionContentLocalizationsRepository HistorySectionContentLocalizationsRepository { get; }

    IRepositoryBase<TEntity> GetRepository<TEntity>()
        where TEntity : class;

    int SaveChanges();

    Task<int> SaveChangesAsync();

    TransactionScope BeginTransaction();
}
