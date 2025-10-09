using System.Reflection;
using System.Transactions;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.Categories;
using VictoryCenter.DAL.Repositories.Interfaces.FaqPlacements;
using VictoryCenter.DAL.Repositories.Interfaces.FaqQuestions;
using VictoryCenter.DAL.Repositories.Interfaces.Media;
using VictoryCenter.DAL.Repositories.Interfaces.ProgramCategories;
using VictoryCenter.DAL.Repositories.Interfaces.Programs;
using VictoryCenter.DAL.Repositories.Interfaces.TeamMembers;
using VictoryCenter.DAL.Repositories.Interfaces.VisitorPages;
using VictoryCenter.DAL.Repositories.Realizations.Categories;
using VictoryCenter.DAL.Repositories.Realizations.FaqPlacements;
using VictoryCenter.DAL.Repositories.Realizations.FaqQuestions;
using VictoryCenter.DAL.Repositories.Realizations.Media;
using VictoryCenter.DAL.Repositories.Realizations.ProgramCategories;
using VictoryCenter.DAL.Repositories.Realizations.Programs;
using VictoryCenter.DAL.Repositories.Realizations.TeamMembers;
using VictoryCenter.DAL.Repositories.Realizations.VisitorPages;

namespace VictoryCenter.DAL.Repositories.Realizations.Base;

public class RepositoryWrapper : IRepositoryWrapper
{
    private readonly VictoryCenterDbContext _victoryCenterDbContext;

    private ICategoriesRepository? _categoriesRepository;
    private IFaqPlacementsRepository? _faqPlacementsRepository;
    private IFaqQuestionsRepository? _faqQuestionsRepository;
    private ITeamMembersRepository? _teamMembersRepository;
    private IVisitorPagesRepository? _visitorPagesRepository;
    private IImageRepository? _imageRepository;
    private IProgramCategoriesRepository? _programCategoriesRepository;
    private IProgramsRepository? _programsRepository;

    public RepositoryWrapper(VictoryCenterDbContext context)
    {
        _victoryCenterDbContext = context;
    }

    public ICategoriesRepository CategoriesRepository => _categoriesRepository ??= new CategoriesRepository(_victoryCenterDbContext);
    public IFaqPlacementsRepository FaqPlacementsRepository => _faqPlacementsRepository ??= new FaqPlacementsRepository(_victoryCenterDbContext);
    public IFaqQuestionsRepository FaqQuestionsRepository => _faqQuestionsRepository ??= new FaqQuestionsRepository(_victoryCenterDbContext);
    public ITeamMembersRepository TeamMembersRepository => _teamMembersRepository ??= new TeamMembersRepository(_victoryCenterDbContext);
    public IVisitorPagesRepository VisitorPagesRepository => _visitorPagesRepository ??= new VisitorPagesRepository(_victoryCenterDbContext);
    public IImageRepository ImageRepository => _imageRepository ??= new ImageRepository(_victoryCenterDbContext);
    public IProgramCategoriesRepository ProgramCategoriesRepository => _programCategoriesRepository
        ??= new ProgramCategoriesRepository(_victoryCenterDbContext);
    public IProgramsRepository ProgramsRepository => _programsRepository ??= new ProgramsRepository(_victoryCenterDbContext);

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
