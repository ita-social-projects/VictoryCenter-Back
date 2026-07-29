using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.Partners;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Localization.Partners;

public class PartnerLocalizationsRepository : RepositoryBase<PartnerLocalization>, IPartnerLocalizationsRepository
{
    public PartnerLocalizationsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
