using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.Partners;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Localization.Partners;

public class PartnerSectionLocalizationsRepository : RepositoryBase<PartnerSectionLocalization>, IPartnerSectionLocalizationsRepository
{
    public PartnerSectionLocalizationsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
