using System.Transactions;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.TeamMembers;
using VictoryCenter.DAL.Repositories.Interfaces.Categories;
using VictoryCenter.DAL.Repositories.Interfaces.Test;
using VictoryCenter.DAL.Repositories.Realizations.TeamMembers;
using VictoryCenter.DAL.Repositories.Realizations.Categories;
using VictoryCenter.DAL.Repositories.Realizations.Test;

namespace VictoryCenter.DAL.Repositories.Realizations.Base;

public class RepositoryWrapper : IRepositoryWrapper
{
    private readonly VictoryCenterDbContext _victoryCenterDbContext;

    private ITestRepository? _testRepository;
    private ICategoriesRepository? _categoriesRepository;
    private ITeamMembersRepository? _teamMembersRepository;

    public RepositoryWrapper(VictoryCenterDbContext context)
    {
        _victoryCenterDbContext = context;
    }

    public ITestRepository TestRepository => _testRepository ??= new TestRepository(_victoryCenterDbContext);
    public ICategoriesRepository CategoriesRepository => _categoriesRepository ??= new CategoriesRepository(_victoryCenterDbContext);
    public ITeamMembersRepository TeamMembersRepository => _teamMembersRepository ??= new TeamMembersRepository(_victoryCenterDbContext);

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
