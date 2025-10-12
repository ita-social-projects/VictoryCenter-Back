using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.Languages;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Localization.Languages;

public class LocalizationLanguagesRepository : RepositoryBase<LocalizationLanguage>, ILocalizationLanguagesRepository
{
    public LocalizationLanguagesRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
