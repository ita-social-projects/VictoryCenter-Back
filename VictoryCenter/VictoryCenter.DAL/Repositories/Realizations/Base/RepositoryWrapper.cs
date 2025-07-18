using System.Transactions;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.TeamMembers;
using VictoryCenter.DAL.Repositories.Interfaces.Categories;
using VictoryCenter.DAL.Repositories.Realizations.TeamMembers;
using VictoryCenter.DAL.Repositories.Realizations.Categories;
using VictoryCenter.DAL.Repositories.Interfaces.FaqPlacements;
using VictoryCenter.DAL.Repositories.Interfaces.FaqQuestions;
using VictoryCenter.DAL.Repositories.Interfaces.VisitorPages;
using VictoryCenter.DAL.Repositories.Realizations.FaqPlacements;
using VictoryCenter.DAL.Repositories.Realizations.FaqQuestions;
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

    public RepositoryWrapper(VictoryCenterDbContext context)
    {
        _victoryCenterDbContext = context;
    }

    public ICategoriesRepository CategoriesRepository => _categoriesRepository ??= new CategoriesRepository(_victoryCenterDbContext);
    public IFaqPlacementsRepository FaqPlacementsRepository => _faqPlacementsRepository ??= new FaqPlacementsRepository(_victoryCenterDbContext);
    public IFaqQuestionsRepository FaqQuestionsRepository => _faqQuestionsRepository ??= new FaqQuestionsRepository(_victoryCenterDbContext);
    public ITeamMembersRepository TeamMembersRepository => _teamMembersRepository ??= new TeamMembersRepository(_victoryCenterDbContext);
    public IVisitorPagesRepository VisitorPagesRepository => _visitorPagesRepository ??= new VisitorPagesRepository(_victoryCenterDbContext);

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
