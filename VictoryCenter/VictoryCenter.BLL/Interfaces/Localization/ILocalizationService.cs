using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.BLL.Interfaces.Localization;

public interface ILocalizationService<TEntity, TEntityLocalization>
    where TEntityLocalization : LocalizationBase<TEntity>
    where TEntity : class, ITranslatedEntity<TEntityLocalization>, IEntity
{
    Task<TEntityLocalization> CreateEntityLocalizationAsync(TEntityLocalization entityLocalization);
    Task<TEntityLocalization> UpdateEntityLocalizationAsync(TEntityLocalization entityLocalization);
    Task<(long entityId, long languageId)> DeleteEntityLocalizationAsync(long entityId, long languageId);
    Task<TEntityLocalization> TrackEntityLocalizationAsync(TEntityLocalization entityLocalization);
    Task TrackEntityLocalizationAsync(IEnumerable<TEntityLocalization> entityLocalization, bool isUpdate);

    Task TrackEntityLocalizationForUpdateAsync(TEntityLocalization entityLocalization);
    Task TrackEntityLocalizationForUpdateAsync(IEnumerable<TEntityLocalization> entityLocalizations);
}
