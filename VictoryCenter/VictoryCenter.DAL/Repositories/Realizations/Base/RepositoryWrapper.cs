using System.Transactions;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.Media;
using VictoryCenter.DAL.Repositories.Interfaces.ProgramCategories;
using VictoryCenter.DAL.Repositories.Interfaces.Programs;
using VictoryCenter.DAL.Repositories.Interfaces.TeamCategories;
using VictoryCenter.DAL.Repositories.Interfaces.TeamMembers;
using VictoryCenter.DAL.Repositories.Realizations.Media;
using VictoryCenter.DAL.Repositories.Realizations.ProgramCategories;
using VictoryCenter.DAL.Repositories.Realizations.Programs;
using VictoryCenter.DAL.Repositories.Realizations.TeamCategories;
using VictoryCenter.DAL.Repositories.Realizations.TeamMembers;

namespace VictoryCenter.DAL.Repositories.Realizations.Base;

public class RepositoryWrapper : IRepositoryWrapper
{
    private readonly VictoryCenterDbContext _victoryCenterDbContext;

    private ITeamCategoriesRepository? _categoriesRepository;
    private ITeamMembersRepository? _teamMembersRepository;
    private IImageRepository? _imageRepository;
    private IProgramCategoriesRepository? _programCategoriesRepository;
    private IProgramsRepository? _programsRepository;

    public RepositoryWrapper(VictoryCenterDbContext context)
    {
        _victoryCenterDbContext = context;
    }

    public ITeamCategoriesRepository TeamCategoriesRepository => _categoriesRepository ??= new TeamCategoriesRepository(_victoryCenterDbContext);
    public ITeamMembersRepository TeamMembersRepository => _teamMembersRepository ??= new TeamMembersRepository(_victoryCenterDbContext);
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
}
