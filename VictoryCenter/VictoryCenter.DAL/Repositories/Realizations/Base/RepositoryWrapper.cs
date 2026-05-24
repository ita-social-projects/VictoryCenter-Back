using System.Reflection;
using System.Transactions;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
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
using VictoryCenter.DAL.Repositories.Realizations.CompanyProfile;
using VictoryCenter.DAL.Repositories.Realizations.Donate;
using VictoryCenter.DAL.Repositories.Realizations.FaqPlacements;
using VictoryCenter.DAL.Repositories.Realizations.FaqQuestions;
using VictoryCenter.DAL.Repositories.Realizations.HippotherapyProgramCategories;
using VictoryCenter.DAL.Repositories.Realizations.HippotherapyPrograms;
using VictoryCenter.DAL.Repositories.Realizations.HistorySections;
using VictoryCenter.DAL.Repositories.Realizations.Localization.CompanyProfile;
using VictoryCenter.DAL.Repositories.Realizations.Localization.FaqQuestions;
using VictoryCenter.DAL.Repositories.Realizations.Localization.HippotherapyPrograms;
using VictoryCenter.DAL.Repositories.Realizations.Localization.Languages;
using VictoryCenter.DAL.Repositories.Realizations.Localization.PdfSection;
using VictoryCenter.DAL.Repositories.Realizations.Localization.ReportFundsExpendituresCategories;
using VictoryCenter.DAL.Repositories.Realizations.Localization.ReportFundsExpendituresSettings;
using VictoryCenter.DAL.Repositories.Realizations.Localization.TeamCategories;
using VictoryCenter.DAL.Repositories.Realizations.Localization.TeamMembers;
using VictoryCenter.DAL.Repositories.Realizations.Localization.WhoWeAreContents;
using VictoryCenter.DAL.Repositories.Realizations.MainPage;
using VictoryCenter.DAL.Repositories.Realizations.Media;
using VictoryCenter.DAL.Repositories.Realizations.Partners;
using VictoryCenter.DAL.Repositories.Realizations.PdfSection;
using VictoryCenter.DAL.Repositories.Realizations.ReportFundsExpendituresCategories;
using VictoryCenter.DAL.Repositories.Realizations.ReportFundsExpendituresRecords;
using VictoryCenter.DAL.Repositories.Realizations.ReportFundsExpendituresSettings;
using VictoryCenter.DAL.Repositories.Realizations.ReportMediaSettings;
using VictoryCenter.DAL.Repositories.Realizations.ReportProgramExpendituresRecords;
using VictoryCenter.DAL.Repositories.Realizations.TeamCategories;
using VictoryCenter.DAL.Repositories.Realizations.TeamMembers;
using VictoryCenter.DAL.Repositories.Realizations.VisitorPages;
using VictoryCenter.DAL.Repositories.Realizations.WhoWeAreContents;
using VictoryCenter.DAL.Repositories.Realizations.WhoWeAreSections;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.MainPage;
using VictoryCenter.DAL.Repositories.Realizations.Localization.MainPage;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.History;
using VictoryCenter.DAL.Repositories.Realizations.Localization.History;

namespace VictoryCenter.DAL.Repositories.Realizations.Base;

public class RepositoryWrapper : IRepositoryWrapper
{
    private readonly VictoryCenterDbContext _victoryCenterDbContext;

    private ITeamCategoriesRepository? _categoriesRepository;
    private IChangedLivesBlockRepository? _changedLivesBlockRepository;
    private ICollectedFundsBlockRepository? _collectedFundsBlockRepository;
    private ICompanyProfileContactLocalizationsRepository? _companyProfileContactLocalizationsRepository;
    private ICompanyProfileContactRepository? _companyProfileContactRepository;
    private ICompanyProfileRepository? _companyProfileRepository;
    private ICompanyProfileRequisiteLocalizationsRepository? _companyProfileRequisiteLocalizationsRepository;
    private ICompanyProfileRequisiteRepository? _companyProfileRequisiteRepository;
    private ICorrespondentBankDetailsRepository? _correspondentBankDetailsRepository;
    private IFaqPlacementsRepository? _faqPlacementsRepository;
    private IFaqQuestionLocalizationsRepository? _faqQuestionLocalizationsRepository;
    private IFaqQuestionsRepository? _faqQuestionsRepository;
    private IForeignBankDetailsRepository? _foreignBankDetailsRepository;
    private IHippotherapyProgramsRepository? _hippotherapyProgramsRepository;
    private IImageRepository? _imageRepository;
    private ILocalizationLanguagesRepository? _localizationLanguagesRepository;
    private IPartnerRepository? _partnerRepository;
    private IPartnerSectionsRepository? _partnerSectionRepository;
    private IPartnersPageBannersRepository? _partnersPageBannersRepository;
    private IPdfReportRepository? _pdfReportRepository;
    private IPdfSectionLocalizationsRepository? _pdfSectionLocalizationsRepository;
    private IPdfSectionRepository? _pdfSectionRepository;
    private IHippotherapyProgramCategoriesRepository? _programCategoriesRepository;
    private IProgramSectionContentLocalizationsRepository? _programSectionContentLocalizationsRepository;
    private IProgramSectionContentsRepository? _programSectionContentsRepository;
    private IHippotherapyProgramsLocalizationsRepository? _programsLocalizationsRepository;
    private IReportFundsExpendituresCategoriesRepository? _reportFundsExpendituresCategoriesRepository;
    private IReportFundsExpendituresRecordsRepository? _reportFundsExpendituresRecordsRepository;
    private IReportFundsExpendituresSettingsRepository? _reportFundsExpendituresSettingsRepository;
    private IReportProgramExpendituresRecordsRepository? _reportProgramExpendituresRecordsRepository;
    private ISupportOptionsRepository? _supportOptionsRepository;
    private ITeamCategoryLocalizationsRepository? _teamCategoryLocalizationsRepository;
    private IReportFundsExpendituresCategoryLocalizationsRepository? _reportFundsExpendituresCategoryLocalizationsRepository;
    private IReportFundsExpendituresSettingsLocalizationsRepository? _reportFundsExpendituresSettingsLocalizationsRepository;
    private ITeamMemberLocalizationsRepository? _teamMemberLocalizationsRepository;
    private ITeamMembersRepository? _teamMembersRepository;
    private IUahBankDetailsRepository? _uahBankDetailsRepository;
    private IVisitorPagesRepository? _visitorPagesRepository;
    private IWhoWeAreContentLocalizationsRepository? _whoWeAreContentLocalizationsRepository;
    private IWhoWeAreContentsRepository? _whoWeAreContentsRepository;
    private IWhoWeAreSectionsRepository? _whoWeAreSectionsRepository;
    private IMainPageRepository? _mainPageRepository;
    private IMainAboutUsRepository? _mainAboutUsRepository;
    private IMainPartnersRepository? _mainPartnersRepository;
    private IMainPageLocalizationsRepository? _mainPageLocalizationsRepository;
    private IMainAboutUsLocalizationsRepository? _mainAboutUsLocalizationsRepository;
    private IMainPartnersLocalizationsRepository? _mainPartnersLocalizationsRepository;
    private IImpactStatisticsRepository? _impactStatisticsRepository;
    private IImpactStatisticsLocalizationsRepository? _impactStatisticsLocalizationsRepository;
    private IMetricRepository? _metricRepository;
    private IMetricLocalizationsRepository? _metricLocalizationsRepository;
    private IHistorySectionsRepository? _historySectionsRepository;
    private IHistorySectionContentsRepository? _historySectionContentsRepository;
    private IHistorySectionContentLocalizationsRepository? _historySectionContentLocalizationsRepository;

    public RepositoryWrapper(VictoryCenterDbContext context)
    {
        _victoryCenterDbContext = context;
    }

    public IFaqPlacementsRepository FaqPlacementsRepository =>
        _faqPlacementsRepository ??= new FaqPlacementsRepository(_victoryCenterDbContext);

    public IFaqQuestionsRepository FaqQuestionsRepository =>
        _faqQuestionsRepository ??= new FaqQuestionsRepository(_victoryCenterDbContext);

    public ITeamCategoriesRepository TeamCategoriesRepository =>
        _categoriesRepository ??= new TeamCategoriesRepository(_victoryCenterDbContext);

    public ITeamMembersRepository TeamMembersRepository =>
        _teamMembersRepository ??= new TeamMembersRepository(_victoryCenterDbContext);

    public IVisitorPagesRepository VisitorPagesRepository =>
        _visitorPagesRepository ??= new VisitorPagesRepository(_victoryCenterDbContext);

    public IImageRepository ImageRepository => _imageRepository ??= new ImageRepository(_victoryCenterDbContext);

    public IPdfReportRepository PdfReportRepository =>
        _pdfReportRepository ??= new PdfReportRepository(_victoryCenterDbContext);

    public IPdfSectionRepository PdfSectionRepository =>
        _pdfSectionRepository ??= new PdfSectionRepository(_victoryCenterDbContext);

    public IPdfSectionLocalizationsRepository PdfSectionLocalizationsRepository =>
        _pdfSectionLocalizationsRepository ??= new PdfSectionLocalizationsRepository(_victoryCenterDbContext);

    public IHippotherapyProgramCategoriesRepository HippotherapyProgramCategoriesRepository =>
        _programCategoriesRepository
            ??= new HippotherapyProgramCategoriesRepository(_victoryCenterDbContext);

    public IHippotherapyProgramsRepository HippotherapyProgramsRepository => _hippotherapyProgramsRepository ??=
        new HippotherapyProgramsRepository(_victoryCenterDbContext);

    public ILocalizationLanguagesRepository LocalizationLanguagesRepository => _localizationLanguagesRepository
        ??= new LocalizationLanguagesRepository(_victoryCenterDbContext);

    public ITeamMemberLocalizationsRepository TeamMemberLocalizationsRepository => _teamMemberLocalizationsRepository
        ??= new TeamMemberLocalizationsRepository(_victoryCenterDbContext);

    public IUahBankDetailsRepository UahBankDetailsRepository => _uahBankDetailsRepository
        ??= new UahBankDetailsRepository(_victoryCenterDbContext);

    public IForeignBankDetailsRepository ForeignBankDetailsRepository => _foreignBankDetailsRepository
        ??= new ForeignBankDetailsRepository(_victoryCenterDbContext);

    public ICorrespondentBankDetailsRepository CorrespondentBankDetailsRepository => _correspondentBankDetailsRepository
        ??= new CorrespondentBankDetailsRepository(_victoryCenterDbContext);

    public ISupportOptionsRepository SupportOptionsRepository => _supportOptionsRepository
        ??= new SupportOptionsRepository(_victoryCenterDbContext);

    public IWhoWeAreContentsRepository WhoWeAreContentsRepository =>
        _whoWeAreContentsRepository ??= new WhoWeAreContentsRepository(_victoryCenterDbContext);

    public IWhoWeAreContentLocalizationsRepository WhoWeAreContentLocalizationsRepository =>
        _whoWeAreContentLocalizationsRepository
            ??= new WhoWeAreContentLocalizationsRepository(_victoryCenterDbContext);

    public IWhoWeAreSectionsRepository WhoWeAreSectionsRepository =>
        _whoWeAreSectionsRepository ??= new WhoWeAreSectionsRepository(_victoryCenterDbContext);

    public IFaqQuestionLocalizationsRepository FaqQuestionLocalizationsRepository => _faqQuestionLocalizationsRepository
        ??= new FaqQuestionLocalizationsRepository(_victoryCenterDbContext);

    public IPartnerRepository PartnerRepository =>
        _partnerRepository ??= new PartnerRepository(_victoryCenterDbContext);

    public IPartnerSectionsRepository PartnerSectionsRepository =>
        _partnerSectionRepository ??= new PartnerSectionsRepository(_victoryCenterDbContext);

    public IPartnersPageBannersRepository PartnersPageBannersRepository => _partnersPageBannersRepository ??=
        new PartnersPageBannersRepository(_victoryCenterDbContext);

    public ITeamCategoryLocalizationsRepository TeamCategoryLocalizationsRepository =>
        _teamCategoryLocalizationsRepository ??= new TeamCategoryLocalizationRepository(_victoryCenterDbContext);

    public IReportFundsExpendituresCategoryLocalizationsRepository ReportFundsExpendituresCategoryLocalizationsRepository =>
        _reportFundsExpendituresCategoryLocalizationsRepository ??=
            new ReportFundsExpendituresCategoryLocalizationsRepository(_victoryCenterDbContext);

    public IReportFundsExpendituresSettingsLocalizationsRepository ReportFundsExpendituresSettingsLocalizationsRepository =>
        _reportFundsExpendituresSettingsLocalizationsRepository ??=
            new ReportFundsExpendituresSettingsLocalizationsRepository(_victoryCenterDbContext);

    public IProgramSectionContentsRepository ProgramSectionContentsRepository => _programSectionContentsRepository ??=
        new ProgramSectionContentsRepository(_victoryCenterDbContext);

    public IProgramSectionContentLocalizationsRepository ProgramSectionContentLocalizationsRepository =>
        _programSectionContentLocalizationsRepository ??=
            new ProgramSectionContentLocalizationsRepository(_victoryCenterDbContext);

    public IHippotherapyProgramsLocalizationsRepository HippotherapyProgramsLocalizationsRepository =>
        _programsLocalizationsRepository
            ??= new HippotherapyProgramsLocalizationsRepository(_victoryCenterDbContext);

    public ICollectedFundsBlockRepository CollectedFundsBlockRepository => _collectedFundsBlockRepository ??=
        new CollectedFundsBlockRepository(_victoryCenterDbContext);

    public IChangedLivesBlockRepository ChangedLivesBlockRepository => _changedLivesBlockRepository ??=
        new ChangedLivesBlockRepository(_victoryCenterDbContext);

    public IReportFundsExpendituresCategoriesRepository ReportFundsExpendituresCategoriesRepository =>
        _reportFundsExpendituresCategoriesRepository ??=
            new ReportFundsExpendituresCategoriesRepository(_victoryCenterDbContext);

    public IReportFundsExpendituresRecordsRepository ReportFundsExpendituresRecordsRepository =>
        _reportFundsExpendituresRecordsRepository ??=
            new ReportFundsExpendituresRecordsRepository(_victoryCenterDbContext);

    public IReportFundsExpendituresSettingsRepository ReportFundsExpendituresSettingsRepository =>
        _reportFundsExpendituresSettingsRepository ??=
            new ReportFundsExpendituresSettingsRepository(_victoryCenterDbContext);

    public IReportProgramExpendituresRecordsRepository ReportProgramExpendituresRecordsRepository =>
        _reportProgramExpendituresRecordsRepository ??=
            new ReportProgramExpendituresRecordsRepository(_victoryCenterDbContext);

    public ICompanyProfileRepository CompanyProfileRepository =>
        _companyProfileRepository ??= new CompanyProfileRepository(_victoryCenterDbContext);

    public ICompanyProfileContactRepository CompanyProfileContactRepository =>
        _companyProfileContactRepository ??= new CompanyProfileContactRepository(_victoryCenterDbContext);

    public ICompanyProfileRequisiteRepository CompanyProfileRequisiteRepository =>
        _companyProfileRequisiteRepository ??= new CompanyProfileRequisiteRepository(_victoryCenterDbContext);

    public ICompanyProfileContactLocalizationsRepository CompanyProfileContactLocalizationsRepository =>
        _companyProfileContactLocalizationsRepository ??=
            new CompanyProfileContactLocalizationsRepository(_victoryCenterDbContext);

    public ICompanyProfileRequisiteLocalizationsRepository CompanyProfileRequisiteLocalizationsRepository =>
        _companyProfileRequisiteLocalizationsRepository ??=
            new CompanyProfileRequisiteLocalizationsRepository(_victoryCenterDbContext);

    public IMainPageRepository MainPageRepository =>
        _mainPageRepository ??= new MainPageRepository(_victoryCenterDbContext);

    public IMainAboutUsRepository MainAboutUsRepository =>
        _mainAboutUsRepository ??= new MainAboutUsRepository(_victoryCenterDbContext);

    public IMainPartnersRepository MainPartnersRepository =>
        _mainPartnersRepository ??= new MainPartnersRepository(_victoryCenterDbContext);

    public IMainPageLocalizationsRepository MainPageLocalizationsRepository =>
        _mainPageLocalizationsRepository ??= new MainPageLocalizationsRepository(_victoryCenterDbContext);

    public IMainAboutUsLocalizationsRepository MainAboutUsLocalizationsRepository =>
        _mainAboutUsLocalizationsRepository ??= new MainAboutUsLocalizationsRepository(_victoryCenterDbContext);

    public IMainPartnersLocalizationsRepository MainPartnersLocalizationsRepository =>
        _mainPartnersLocalizationsRepository ??= new MainPartnersLocalizationsRepository(_victoryCenterDbContext);

    public IImpactStatisticsRepository ImpactStatisticsRepository =>
        _impactStatisticsRepository ??= new ImpactStatisticsRepository(_victoryCenterDbContext);

    public IImpactStatisticsLocalizationsRepository ImpactStatisticsLocalizationsRepository =>
        _impactStatisticsLocalizationsRepository ??= new ImpactStatisticsLocalizationsRepository(_victoryCenterDbContext);

    public IMetricRepository MetricRepository =>
        _metricRepository ??= new MetricRepository(_victoryCenterDbContext);

    public IMetricLocalizationsRepository MetricLocalizationsRepository =>
        _metricLocalizationsRepository ??= new MetricLocalizationsRepository(_victoryCenterDbContext);

    public IHistorySectionsRepository HistorySectionsRepository =>
        _historySectionsRepository ??= new HistorySectionsRepository(_victoryCenterDbContext);
    public IHistorySectionContentsRepository HistorySectionContentsRepository =>
        _historySectionContentsRepository ??= new HistorySectionContentsRepository(_victoryCenterDbContext);

    public IHistorySectionContentLocalizationsRepository HistorySectionContentLocalizationsRepository =>
        _historySectionContentLocalizationsRepository ??= new HistorySectionContentLocalizationsRepository(_victoryCenterDbContext);

    public int SaveChanges()
    {
        return _victoryCenterDbContext.SaveChanges();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _victoryCenterDbContext.SaveChangesAsync();
    }

    public TransactionScope BeginTransaction()
    {
        return new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
    }

    public IRepositoryBase<TEntity> GetRepository<TEntity>()
        where TEntity : class
    {
        var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            var propertyValue = property.GetValue(this);

            if (propertyValue is IRepositoryBase<TEntity> matchingRepository)
            {
                return matchingRepository;
            }
        }

        throw new NotImplementedException(
            $"Repository for entity type '{typeof(TEntity).Name}' is not found in {nameof(RepositoryWrapper)}.");
    }
}
