using FluentResults;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.EventNews;
using VictoryCenter.BLL.Interfaces.SlugService;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using EventNewsEntity = VictoryCenter.DAL.Entities.EventNews;

namespace VictoryCenter.BLL.Helpers;

public static class EventNewsAggregateHelper
{
    private const int MaxSlugSaveAttempts = 3;

    public static async Task<Result<IReadOnlyDictionary<long, LocalizationLanguage>>> ValidateAndGetLanguagesAsync(
        IRepositoryWrapper repositoryWrapper,
        IEnumerable<CreateEventNewsLocalizationDto> localizations)
    {
        var languageIds = localizations
            .Select(localization => localization.LanguageId)
            .Distinct()
            .ToList();

        if (languageIds.Count == 0)
        {
            return Result.Ok<IReadOnlyDictionary<long, LocalizationLanguage>>(
                new Dictionary<long, LocalizationLanguage>());
        }

        var languages = (await repositoryWrapper.LocalizationLanguagesRepository.GetAllAsync(
            new QueryOptions<LocalizationLanguage>
            {
                Filter = language => languageIds.Contains(language.Id),
                AsNoTracking = false
            })).ToList();

        var languagesById = languages.ToDictionary(language => language.Id);
        var missingIds = languageIds.Where(id => !languagesById.ContainsKey(id)).ToList();

        return missingIds.Count == 0
            ? Result.Ok<IReadOnlyDictionary<long, LocalizationLanguage>>(languagesById)
            : Result.Fail<IReadOnlyDictionary<long, LocalizationLanguage>>(
                ErrorMessagesConstants.NotFound(missingIds, typeof(LocalizationLanguage)));
    }

    public static async Task<int> SaveWithSlugRetryAsync(
        IRepositoryWrapper repositoryWrapper,
        ISlugService slugService,
        EventNewsEntity eventNews,
        string? titleForSlug,
        CancellationToken cancellationToken)
    {
        for (var attempt = 1; attempt <= MaxSlugSaveAttempts; attempt++)
        {
            try
            {
                return await repositoryWrapper.SaveChangesAsync();
            }
            catch (DbUpdateException exception) when (
                attempt < MaxSlugSaveAttempts
                && !string.IsNullOrWhiteSpace(titleForSlug)
                && exception.IsUniqueConstraintException())
            {
                var slugAlreadyExists = !string.IsNullOrWhiteSpace(eventNews.Slug)
                    && await repositoryWrapper.EventNewsRepository.ExistsAsync(
                        existingEventNews => existingEventNews.Id != eventNews.Id
                            && existingEventNews.Slug == eventNews.Slug);

                if (!slugAlreadyExists)
                {
                    throw;
                }

                eventNews.Slug = await slugService.GenerateUniqueEventNewsSlugAsync(
                    eventNews.Id,
                    titleForSlug,
                    cancellationToken);
            }
        }

        return 0;
    }
}
