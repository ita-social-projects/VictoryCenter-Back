using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Localization;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Localization;
public class LocalizationLanguagesRepository : RepositoryBase<LocalizationLanguage>, ILocalizationLanguagesRepository
{
    public LocalizationLanguagesRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
