using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Services.Localization;

public class LocalizationService<TEntity, TEntityLocalization> : ILocalizationService<TEntity, TEntityLocalization>
    where TEntityLocalization : LocalizationBase<TEntity>
    where TEntity : class, ITranslatedEntity<TEntityLocalization>, IEntity
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public LocalizationService(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<TEntityLocalization> CreateEntityLocalizationAsync(TEntityLocalization entityLocalization)
    {
        var entity = await _repositoryWrapper.GetRepository<TEntity>()
            .GetFirstOrDefaultAsync(
                new QueryOptions<TEntity>
                {
                    Filter = e => e.Id == entityLocalization.EntityId,
                });

        if (entity is null)
        {
            throw new KeyNotFoundException(ErrorMessagesConstants.NotFound(entityLocalization.EntityId, typeof(TEntity)));
        }

        var localizationLanguage = await _repositoryWrapper.LocalizationLanguagesRepository
            .GetFirstOrDefaultAsync(
            new QueryOptions<LocalizationLanguage>
            {
                Filter = e => e.Id == entityLocalization.LanguageId,
            });

        if (localizationLanguage is null)
        {
            throw new KeyNotFoundException(ErrorMessagesConstants.NotFound(entityLocalization.LanguageId, typeof(LocalizationLanguage)));
        }

        entityLocalization.CreatedAt = DateTimeOffset.UtcNow;
        var createdEntity = await _repositoryWrapper.GetRepository<TEntityLocalization>().CreateAsync(entityLocalization);

        if (await _repositoryWrapper.SaveChangesAsync() > 0)
        {
            var resultWithLanguage = await _repositoryWrapper.GetRepository<TEntityLocalization>()
            .GetFirstOrDefaultAsync(new QueryOptions<TEntityLocalization>
            {
                Filter = l => l.EntityId == createdEntity.EntityId && l.LanguageId == createdEntity.LanguageId,
                Include = l => l.Include(x => x.Language)
            });

            if (resultWithLanguage is null)
            {
                throw new InvalidOperationException("Failed to retrieve created localization with language.");
            }

            return resultWithLanguage;
        }

        throw new InvalidOperationException();
    }

    public async Task<(long entityId, long languageId)> DeleteEntityLocalizationAsync(long entityId, long languageId)
    {
        TEntityLocalization? entityToDelete = await _repositoryWrapper.GetRepository<TEntityLocalization>()
            .GetFirstOrDefaultAsync(new QueryOptions<TEntityLocalization>
            {
                Filter = localization => localization.EntityId == entityId &&
                                           localization.LanguageId == languageId
            });

        if (entityToDelete is null)
        {
            throw new KeyNotFoundException(ErrorMessagesConstants.NotFound((entityId, languageId), typeof(TEntityLocalization)));
        }

        _repositoryWrapper.GetRepository<TEntityLocalization>().Delete(entityToDelete);

        if (await _repositoryWrapper.SaveChangesAsync() > 0)
        {
            return (entityId, languageId);
        }

        throw new InvalidOperationException();
    }

    public async Task<TEntityLocalization> UpdateEntityLocalizationAsync(TEntityLocalization entityLocalization)
    {
        TEntityLocalization? entityToUpdate = await _repositoryWrapper.GetRepository<TEntityLocalization>()
                .GetFirstOrDefaultAsync(new QueryOptions<TEntityLocalization>
                {
                    Filter = localization => localization.EntityId == entityLocalization.EntityId &&
                                           localization.LanguageId == entityLocalization.LanguageId,
                    Include = localization => localization
                        .Include(l => l.Language)
                });

        if (entityToUpdate is null)
        {
            throw new KeyNotFoundException(ErrorMessagesConstants.NotFound((entityLocalization.EntityId, entityLocalization.LanguageId), typeof(TEntityLocalization)));
        }

        entityLocalization.TranslationStatus = TranslationStatus.Relevant;

        _repositoryWrapper.GetRepository<TEntityLocalization>().Update(entityLocalization);

        if (await _repositoryWrapper.SaveChangesAsync() > 0)
        {
            return entityLocalization;
        }

        throw new InvalidOperationException();
    }
}
