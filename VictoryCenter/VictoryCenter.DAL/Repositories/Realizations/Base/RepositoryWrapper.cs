using System.Reflection;
using System.Transactions;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
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
using VictoryCenter.DAL.Repositories.Realizations.Donate;
using VictoryCenter.DAL.Repositories.Realizations.FaqPlacements;
using VictoryCenter.DAL.Repositories.Realizations.FaqQuestions;
using VictoryCenter.DAL.Repositories.Realizations.Localization.Languages;
using VictoryCenter.DAL.Repositories.Realizations.Localization.TeamMembers;
using VictoryCenter.DAL.Repositories.Realizations.Donate;
using VictoryCenter.DAL.Repositories.Realizations.Media;
using VictoryCenter.DAL.Repositories.Realizations.TeamCategories;
using VictoryCenter.DAL.Repositories.Realizations.TeamMembers;
using VictoryCenter.DAL.Repositories.Realizations.VisitorPages;
using VictoryCenter.DAL.Repositories.Realizations.WhoWeAreContents;
using VictoryCenter.DAL.Repositories.Realizations.WhoWeAreSections;

namespace VictoryCenter.DAL.Repositories.Realizations.Base;

public class RepositoryWrapper : IRepositoryWrapper
{
    private readonly VictoryCenterDbContext _victoryCenterDbContext;

    private ITeamCategoriesRepository? _categoriesRepository;
    private IFaqPlacementsRepository? _faqPlacementsRepository;
    private IFaqQuestionsRepository? _faqQuestionsRepository;
    private ITeamMembersRepository? _teamMembersRepository;
    private IVisitorPagesRepository? _visitorPagesRepository;
    private IImageRepository? _imageRepository;
    private IHippotherapyProgramCategoriesRepository? _programCategoriesRepository;
    private IHippotherapyProgramsRepository? _hippotherapyProgramsRepository;
    private ILocalizationLanguagesRepository? _localizationLanguagesRepository;
    private ITeamMemberLocalizationsRepository? _teamMemberLocalizationsRepository;
    private IUahBankDetailsRepository? _uahBankDetailsRepository;
    private IForeignBankDetailsRepository? _foreignBankDetailsRepository;
    private ICorrespondentBankDetailsRepository? _correspondentBankDetailsRepository;
    private ISupportOptionsRepository? _supportOptionsRepository;
    private IWhoWeAreContentsRepository? _whoWeAreContentsRepository;
    private IWhoWeAreSectionsRepository? _whoWeAreSectionsRepository;

    public RepositoryWrapper(VictoryCenterDbContext context)
    {
        _victoryCenterDbContext = context;
    }

    public IFaqPlacementsRepository FaqPlacementsRepository => _faqPlacementsRepository ??= new FaqPlacementsRepository(_victoryCenterDbContext);
    public IFaqQuestionsRepository FaqQuestionsRepository => _faqQuestionsRepository ??= new FaqQuestionsRepository(_victoryCenterDbContext);
    public ITeamCategoriesRepository TeamCategoriesRepository => _categoriesRepository ??= new TeamCategoriesRepository(_victoryCenterDbContext);
    public ITeamMembersRepository TeamMembersRepository => _teamMembersRepository ??= new TeamMembersRepository(_victoryCenterDbContext);
    public IVisitorPagesRepository VisitorPagesRepository => _visitorPagesRepository ??= new VisitorPagesRepository(_victoryCenterDbContext);
    public IImageRepository ImageRepository => _imageRepository ??= new ImageRepository(_victoryCenterDbContext);
    public IHippotherapyProgramCategoriesRepository HippotherapyProgramCategoriesRepository => _programCategoriesRepository
        ??= new HippotherapyProgramCategoriesRepository(_victoryCenterDbContext);
    public IHippotherapyProgramsRepository HippotherapyProgramsRepository => _hippotherapyProgramsRepository ??= new HippotherapyProgramsRepository(_victoryCenterDbContext);
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
    public IWhoWeAreContentsRepository WhoWeAreContentsRepository => _whoWeAreContentsRepository ??= new WhoWeAreContentsRepository(_victoryCenterDbContext);
    public IWhoWeAreSectionsRepository WhoWeAreSectionsRepository => _whoWeAreSectionsRepository ??= new WhoWeAreSectionsRepository(_victoryCenterDbContext);

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

        throw new NotImplementedException($"Repository for entity type '{typeof(TEntity).Name}' is not found in {nameof(RepositoryWrapper)}.");
    }
}
