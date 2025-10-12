using System.Reflection;
using System.Transactions;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.FaqPlacements;
using VictoryCenter.DAL.Repositories.Interfaces.FaqQuestions;
using VictoryCenter.DAL.Repositories.Interfaces.HypotherapyProgramCategories;
using VictoryCenter.DAL.Repositories.Interfaces.HypotherapyPrograms;
using VictoryCenter.DAL.Repositories.Interfaces.Media;
using VictoryCenter.DAL.Repositories.Interfaces.TeamCategories;
using VictoryCenter.DAL.Repositories.Interfaces.TeamMembers;
using VictoryCenter.DAL.Repositories.Interfaces.VisitorPages;
using VictoryCenter.DAL.Repositories.Realizations.FaqPlacements;
using VictoryCenter.DAL.Repositories.Realizations.FaqQuestions;
using VictoryCenter.DAL.Repositories.Realizations.HypotherapyProgramCategories;
using VictoryCenter.DAL.Repositories.Realizations.HypotherapyPrograms;
using VictoryCenter.DAL.Repositories.Realizations.Media;
using VictoryCenter.DAL.Repositories.Realizations.TeamCategories;
using VictoryCenter.DAL.Repositories.Realizations.TeamMembers;
using VictoryCenter.DAL.Repositories.Realizations.VisitorPages;

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
    private IHypotherapyProgramCategoriesRepository? _programCategoriesRepository;
    private IHypotherapyProgramsRepository? _hypotherapyProgramsRepository;

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
    public IHypotherapyProgramCategoriesRepository HypotherapyProgramCategoriesRepository => _programCategoriesRepository
        ??= new HypotherapyProgramCategoriesRepository(_victoryCenterDbContext);
    public IHypotherapyProgramsRepository HypotherapyProgramsRepository => _hypotherapyProgramsRepository ??= new HypotherapyProgramsRepository(_victoryCenterDbContext);

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
