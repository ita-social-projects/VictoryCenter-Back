using System.Transactions;
using VictoryCenter.DAL.Repositories.Interfaces.Categories;
using VictoryCenter.DAL.Repositories.Interfaces.Donations;
using VictoryCenter.DAL.Repositories.Interfaces.Media;
using VictoryCenter.DAL.Repositories.Interfaces.TeamMembers;

namespace VictoryCenter.DAL.Repositories.Interfaces.Base;

public interface IRepositoryWrapper
{
    ICategoriesRepository CategoriesRepository { get; }
    ITeamMembersRepository TeamMembersRepository { get; }
    IImageRepository ImageRepository { get; }
    IAdditionalFieldRepository AdditionalFieldRepository { get; }
    ICorrespondentBankRepository CorrespondentBankRepository { get; }
    IForeignBankDetailsRepository ForeignBankDetailsRepository { get; }
    ISupportMethodRepository SupportMethodRepository { get; }
    IUahBankDetailsRepository UahBankDetailsRepository { get; }

    int SaveChanges();

    Task<int> SaveChangesAsync();

    TransactionScope BeginTransaction();
}
