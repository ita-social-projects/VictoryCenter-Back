using System.Transactions;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.TeamMembers;
using VictoryCenter.DAL.Repositories.Interfaces.Categories;
using VictoryCenter.DAL.Repositories.Interfaces.Donations;
using VictoryCenter.DAL.Repositories.Interfaces.Media;
using VictoryCenter.DAL.Repositories.Realizations.TeamMembers;
using VictoryCenter.DAL.Repositories.Realizations.Categories;
using VictoryCenter.DAL.Repositories.Realizations.Donations;
using VictoryCenter.DAL.Repositories.Realizations.Media;

namespace VictoryCenter.DAL.Repositories.Realizations.Base;

public class RepositoryWrapper : IRepositoryWrapper
{
    private readonly VictoryCenterDbContext _victoryCenterDbContext;

    private ICategoriesRepository? _categoriesRepository;
    private ITeamMembersRepository? _teamMembersRepository;
    private IImageRepository? _imageRepository;
    private IAdditionalFieldRepository? _additionalFieldRepository;
    private ICorrespondentBankRepository? _correspondentBankRepository;
    private IForeignBankDetailsRepository? _foreignBankDetailsRepository;
    private ISupportMethodRepository? _supportMethodRepository;
    private IUahBankDetailsRepository? _uahBankDetailsRepository;

    public RepositoryWrapper(VictoryCenterDbContext context)
    {
        _victoryCenterDbContext = context;
    }

    public ICategoriesRepository CategoriesRepository => _categoriesRepository ??= new CategoriesRepository(_victoryCenterDbContext);
    public ITeamMembersRepository TeamMembersRepository => _teamMembersRepository ??= new TeamMembersRepository(_victoryCenterDbContext);
    public IImageRepository ImageRepository => _imageRepository ??= new ImageRepository(_victoryCenterDbContext);
    public IAdditionalFieldRepository AdditionalFieldRepository => _additionalFieldRepository ??= new AdditionalFieldRepository(_victoryCenterDbContext);
    public ICorrespondentBankRepository CorrespondentBankRepository => _correspondentBankRepository ??= new CorrespondentBankRepository(_victoryCenterDbContext);
    public IForeignBankDetailsRepository ForeignBankDetailsRepository => _foreignBankDetailsRepository ??= new ForeignBankDetailsRepository(_victoryCenterDbContext);
    public ISupportMethodRepository SupportMethodRepository => _supportMethodRepository ??= new SupportMethodRepository(_victoryCenterDbContext);
    public IUahBankDetailsRepository UahBankDetailsRepository => _uahBankDetailsRepository ??= new UahBankDetailsRepository(_victoryCenterDbContext);

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
