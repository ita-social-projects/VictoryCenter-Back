using System.Transactions;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Repositories.Interfaces.AboutUsContents;
using VictoryCenter.DAL.Repositories.Interfaces.AboutUsSections;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.TeamMembers;
using VictoryCenter.DAL.Repositories.Interfaces.Categories;
using VictoryCenter.DAL.Repositories.Interfaces.Media;
using VictoryCenter.DAL.Repositories.Realizations.AboutUsContents;
using VictoryCenter.DAL.Repositories.Realizations.AboutUsSections;
using VictoryCenter.DAL.Repositories.Realizations.TeamMembers;
using VictoryCenter.DAL.Repositories.Realizations.Categories;
using VictoryCenter.DAL.Repositories.Realizations.Media;

namespace VictoryCenter.DAL.Repositories.Realizations.Base;

public class RepositoryWrapper : IRepositoryWrapper
{
    private readonly VictoryCenterDbContext _victoryCenterDbContext;

    private ICategoriesRepository? _categoriesRepository;
    private ITeamMembersRepository? _teamMembersRepository;
    private IImageRepository? _imageRepository;
    private IAboutUsContentsRepository? _aboutUsContentsRepository;
    private IAboutUsSectionsRepository? _aboutUsSectionsRepository;

    public RepositoryWrapper(VictoryCenterDbContext context)
    {
        _victoryCenterDbContext = context;
    }

    public ICategoriesRepository CategoriesRepository => _categoriesRepository ??= new CategoriesRepository(_victoryCenterDbContext);
    public ITeamMembersRepository TeamMembersRepository => _teamMembersRepository ??= new TeamMembersRepository(_victoryCenterDbContext);
    public IImageRepository ImageRepository => _imageRepository ??= new ImageRepository(_victoryCenterDbContext);

    public IAboutUsContentsRepository AboutUsContentsRepository => _aboutUsContentsRepository ??= new AboutUsContentsRepository(_victoryCenterDbContext);

    public IAboutUsSectionsRepository AboutUsSectionsRepository => _aboutUsSectionsRepository ??= new AboutUsSectionsRepository(_victoryCenterDbContext);

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
