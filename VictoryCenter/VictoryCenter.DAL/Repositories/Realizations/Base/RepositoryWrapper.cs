using System.Transactions;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.TeamMembers;
using VictoryCenter.DAL.Repositories.Interfaces.Categories;
using VictoryCenter.DAL.Repositories.Interfaces.Media;
using VictoryCenter.DAL.Repositories.Interfaces.ProgramCategories;
using VictoryCenter.DAL.Repositories.Interfaces.Programs;
using VictoryCenter.DAL.Repositories.Interfaces.WhoWeAreContents;
using VictoryCenter.DAL.Repositories.Interfaces.WhoWeAreSections;
using VictoryCenter.DAL.Repositories.Realizations.TeamMembers;
using VictoryCenter.DAL.Repositories.Realizations.Categories;
using VictoryCenter.DAL.Repositories.Realizations.Media;
using VictoryCenter.DAL.Repositories.Realizations.ProgramCategories;
using VictoryCenter.DAL.Repositories.Realizations.Programs;
using VictoryCenter.DAL.Repositories.Realizations.WhoWeAreContents;
using VictoryCenter.DAL.Repositories.Realizations.WhoWeAreSections;

namespace VictoryCenter.DAL.Repositories.Realizations.Base;

public class RepositoryWrapper : IRepositoryWrapper
{
    private readonly VictoryCenterDbContext _victoryCenterDbContext;

    private ICategoriesRepository? _categoriesRepository;
    private ITeamMembersRepository? _teamMembersRepository;
    private IImageRepository? _imageRepository;
    private IWhoWeAreContentsRepository? _whoWeAreContentsRepository;
    private IWhoWeAreSectionsRepository? _whoWeAreSectionsRepository;
    private IProgramsRepository? _programsRepository;
    private IProgramCategoriesRepository? _programCategoriesRepository;

    public RepositoryWrapper(VictoryCenterDbContext context)
    {
        _victoryCenterDbContext = context;
    }

    public ICategoriesRepository CategoriesRepository => _categoriesRepository ??= new CategoriesRepository(_victoryCenterDbContext);
    public ITeamMembersRepository TeamMembersRepository => _teamMembersRepository ??= new TeamMembersRepository(_victoryCenterDbContext);
    public IImageRepository ImageRepository => _imageRepository ??= new ImageRepository(_victoryCenterDbContext);

    public IWhoWeAreContentsRepository WhoWeAreContentsRepository => _whoWeAreContentsRepository ??= new WhoWeAreContentsRepository(_victoryCenterDbContext);

    public IWhoWeAreSectionsRepository WhoWeAreSectionsRepository => _whoWeAreSectionsRepository ??= new WhoWeAreSectionsRepository(_victoryCenterDbContext);

    public IProgramsRepository ProgramsRepository => _programsRepository ??= new ProgramsRepository(_victoryCenterDbContext);

    public IProgramCategoriesRepository ProgramCategoriesRepository => _programCategoriesRepository ??= new ProgramCategoriesRepository(_victoryCenterDbContext);
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
}
