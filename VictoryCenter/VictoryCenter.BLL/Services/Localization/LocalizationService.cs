using FluentValidation;
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
        var createdEntity = await ValidateAndTrackAsync(entityLocalization);

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

    public async Task<TEntityLocalization> TrackEntityLocalizationAsync(TEntityLocalization entityLocalization)
    {
        var createdEntity = await ValidateAndTrackAsync(entityLocalization);

        return createdEntity;
    }

    public async Task TrackEntityLocalizationAsync(IEnumerable<TEntityLocalization> localizations)
    {
        var entityLocalizations = localizations.ToList();
        var entityIds = entityLocalizations
            .Select(l => l.EntityId)
            .Distinct()
            .ToList();

        var existingEntityIds = (await _repositoryWrapper
            .GetRepository<TEntity>()
            .GetAllAsync(new QueryOptions<TEntity>()
            {
                Filter = entity => entityIds.Contains(entity.Id)
            }))
            .Select(e => e.Id)
            .ToList();

        var missingEntityIds = entityIds.Except(existingEntityIds).ToList();

        if (missingEntityIds.Count > 0)
        {
            throw new KeyNotFoundException(ErrorMessagesConstants.NotFound(missingEntityIds.First(), typeof(TEntity)));
        }

        if (entityLocalizations.Select(x => x.LanguageId).Distinct().Count() != 1)
        {
            throw new ValidationException("Bulk localization supports only one LanguageId.");
        }

        var languageId = entityLocalizations.First().LanguageId;

        var languageExists = await _repositoryWrapper
            .LocalizationLanguagesRepository
            .GetFirstOrDefaultAsync(new QueryOptions<LocalizationLanguage>()
            {
                Filter = l => l.Id == languageId
            });

        if (languageExists is null)
        {
            throw new KeyNotFoundException(ErrorMessagesConstants.NotFound(languageId, typeof(LocalizationLanguage)));
        }

        await _repositoryWrapper.GetRepository<TEntityLocalization>()
            .CreateRangeAsync(entityLocalizations);
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

        var updatedEntity = _repositoryWrapper.GetRepository<TEntityLocalization>().Update(entityLocalization);

        if (await _repositoryWrapper.SaveChangesAsync() > 0)
        {
            var resultWithLanguage = await _repositoryWrapper.GetRepository<TEntityLocalization>()
                .GetFirstOrDefaultAsync(new QueryOptions<TEntityLocalization>
                {
                    Filter = l => l.EntityId == updatedEntity.Entity.EntityId && l.LanguageId == updatedEntity.Entity.LanguageId,
                    Include = l => l.Include(x => x.Language)
                });

            if (resultWithLanguage is null)
            {
                throw new InvalidOperationException("Failed to retrieve updated localization with language.");
            }

            return resultWithLanguage;
        }

        throw new InvalidOperationException();
    }

    public async Task TrackEntityLocalizationForUpdateAsync(TEntityLocalization entityLocalization)
    {
        var existingLocalization = await _repositoryWrapper.GetRepository<TEntityLocalization>()
            .GetFirstOrDefaultAsync(new QueryOptions<TEntityLocalization>
            {
                Filter = localization => localization.EntityId == entityLocalization.EntityId
                                     && localization.LanguageId == entityLocalization.LanguageId
            });

        if (existingLocalization is null)
        {
            throw new KeyNotFoundException(
                ErrorMessagesConstants.NotFound((entityLocalization.EntityId, entityLocalization.LanguageId), typeof(TEntityLocalization)));
        }

        entityLocalization.TranslationStatus = TranslationStatus.Relevant;

        _repositoryWrapper.GetRepository<TEntityLocalization>().Update(entityLocalization);
    }

    public async Task TrackEntityLocalizationForUpdateAsync(IEnumerable<TEntityLocalization> entityLocalizations)
    {
        var localizations = entityLocalizations.ToList();

        if (localizations.Count == 0)
        {
            return;
        }

        if (localizations.Select(x => x.LanguageId).Distinct().Count() != 1)
        {
            throw new ValidationException("Bulk localization update supports only one LanguageId.");
        }

        var languageId = localizations.First().LanguageId;
        var entityIds = localizations
            .Select(x => x.EntityId)
            .Distinct()
            .ToList();

        var existingLocalizations = (await _repositoryWrapper.GetRepository<TEntityLocalization>()
            .GetAllAsync(new QueryOptions<TEntityLocalization>
            {
                Filter = localization => localization.LanguageId == languageId
                                     && entityIds.Contains(localization.EntityId)
            }))
            .ToList();

        var existingIds = existingLocalizations
            .Select(x => x.EntityId)
            .ToHashSet();

        var missingId = entityIds.FirstOrDefault(id => !existingIds.Contains(id));

        if (missingId != 0)
        {
            throw new KeyNotFoundException(
                ErrorMessagesConstants.NotFound((missingId, languageId), typeof(TEntityLocalization)));
        }

        foreach (var localization in localizations)
        {
            localization.TranslationStatus = TranslationStatus.Relevant;
        }

        _repositoryWrapper.GetRepository<TEntityLocalization>().UpdateRange(localizations);
    }

    private async Task<TEntityLocalization> ValidateAndTrackAsync(TEntityLocalization entityLocalization)
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

        return await _repositoryWrapper.GetRepository<TEntityLocalization>().CreateAsync(entityLocalization);
    }
}
