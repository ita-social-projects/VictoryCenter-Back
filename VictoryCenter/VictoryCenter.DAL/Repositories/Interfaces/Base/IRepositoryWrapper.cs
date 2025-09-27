using System.Transactions;
using VictoryCenter.DAL.Repositories.Interfaces.Categories;
using VictoryCenter.DAL.Repositories.Interfaces.Donate;
using VictoryCenter.DAL.Repositories.Interfaces.Media;
using VictoryCenter.DAL.Repositories.Interfaces.ProgramCategories;
using VictoryCenter.DAL.Repositories.Interfaces.Programs;
using VictoryCenter.DAL.Repositories.Interfaces.TeamMembers;

namespace VictoryCenter.DAL.Repositories.Interfaces.Base;

public interface IRepositoryWrapper
{
    ICategoriesRepository CategoriesRepository { get; }
    ITeamMembersRepository TeamMembersRepository { get; }
    IImageRepository ImageRepository { get; }
    IProgramCategoriesRepository ProgramCategoriesRepository { get; }
    IProgramsRepository ProgramsRepository { get; }
    IUahBankDetailsRepository UahBankDetailsRepository { get; }
    IForeignBankDetailsRepository ForeignBankDetailsRepository { get; }
    ICorrespondentBankDetailsRepository CorrespondentBankDetailsRepository { get; }
    ISupportOptionsRepository SupportOptionsRepository { get; }

    int SaveChanges();

    Task<int> SaveChangesAsync();

    TransactionScope BeginTransaction();
}
