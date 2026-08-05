using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.Partners;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Localization.Partners;

public class PartnersPageBannerLocalizationsRepository : RepositoryBase<PartnersPageBannerLocalization>, IPartnersPageBannerLocalizationsRepository
{
    public PartnersPageBannerLocalizationsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
