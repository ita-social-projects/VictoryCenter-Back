using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.EventNews;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.BLL.Interfaces.SlugService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using EventNewsEntity = VictoryCenter.DAL.Entities.EventNews;

namespace VictoryCenter.BLL.Commands.Admin.EventNews.Update;

public class UpdateEventNewsHandler : IRequestHandler<UpdateEventNewsCommand, Result<EventNewsDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ISlugService _slugService;

    public UpdateEventNewsHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        ISlugService slugService)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _slugService = slugService;
    }

    public async Task<Result<EventNewsDto>> Handle(
        UpdateEventNewsCommand request,
        CancellationToken cancellationToken)
    {
        var eventNews = await GetEventNewsAsync(request.Id);
        if (eventNews is null)
        {
            return Result.Fail<EventNewsDto>(
                ErrorMessagesConstants.NotFound(request.Id, typeof(EventNewsEntity)));
        }

        var dto = request.EventNews;
        var categoryIds = dto.CategoryIds ?? [];
        var localizationDtos = (dto.Localizations ?? [])
            .Where(localization => localization is not null && !string.IsNullOrWhiteSpace(localization.Title))
            .ToList();

        var categoriesResult = await CategoryValidationHelper.ValidateAndGetCategoriesAsync(
            _repositoryWrapper.EventNewsCategoryRepository,
            categoryIds,
            query => query
                .Include(category => category.Localizations)
                .ThenInclude(localization => localization.Language));

        if (categoriesResult.IsFailed)
        {
            return Result.Fail<EventNewsDto>(categoriesResult.Errors);
        }

        var imagesResult = await ImageValidationHelper.ValidateAndGetImagesByIdsAsync(
            _repositoryWrapper,
            GetRequestedImageIds(dto));

        if (imagesResult.IsFailed)
        {
            return Result.Fail<EventNewsDto>(imagesResult.Errors);
        }

        var languagesResult = await EventNewsAggregateHelper.ValidateAndGetLanguagesAsync(
            _repositoryWrapper,
            localizationDtos);
        if (languagesResult.IsFailed)
        {
            return Result.Fail<EventNewsDto>(languagesResult.Errors);
        }

        var categoriesChanged = !HaveSameCategoryIds(eventNews.Categories, categoryIds);
        var localizationsChanged = HaveLocalizationChanges(eventNews.Localizations, localizationDtos);
        var titlesChanged = HaveLocalizationTitlesChanged(eventNews.Localizations, localizationDtos);
        var slugStateChanged = string.IsNullOrWhiteSpace(eventNews.Slug) != (localizationDtos.Count == 0);
        var hasChanges = HasScalarOrImageChanges(eventNews, dto)
                         || categoriesChanged
                         || localizationsChanged
                         || slugStateChanged;

        if (!hasChanges)
        {
            return Result.Ok(_mapper.Map<EventNewsDto>(eventNews));
        }

        ApplyScalarAndImageChanges(eventNews, dto, imagesResult.Value);

        if (categoriesChanged)
        {
            ReplaceCategories(eventNews, categoriesResult.Value);
        }

        if (localizationsChanged)
        {
            MergeLocalizations(eventNews, localizationDtos, languagesResult.Value);
        }

        var titleForSlug = localizationDtos
            .Select(localization => localization.Title?.Trim())
            .FirstOrDefault(title => !string.IsNullOrWhiteSpace(title));

        if (string.IsNullOrWhiteSpace(titleForSlug))
        {
            eventNews.Slug = null;
        }
        else if (titlesChanged || string.IsNullOrWhiteSpace(eventNews.Slug))
        {
            eventNews.Slug = await _slugService.GenerateUniqueEventNewsSlugAsync(
                eventNews.Id,
                titleForSlug,
                cancellationToken);
        }

        try
        {
            if (await EventNewsAggregateHelper.SaveWithSlugRetryAsync(
                    _repositoryWrapper,
                    _slugService,
                    eventNews,
                    titleForSlug,
                    cancellationToken) > 0)
            {
                return Result.Ok(_mapper.Map<EventNewsDto>(eventNews));
            }
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result.Fail<EventNewsDto>(
                ErrorMessagesConstants.NotFound(request.Id, typeof(EventNewsEntity)));
        }

        return Result.Fail<EventNewsDto>(
            ErrorMessagesConstants.FailedToUpdateEntity(typeof(EventNewsEntity)));
    }

    private async Task<EventNewsEntity?> GetEventNewsAsync(long id)
    {
        return await _repositoryWrapper.EventNewsRepository.GetFirstOrDefaultAsync(
            new QueryOptions<EventNewsEntity>
            {
                Filter = eventNews => eventNews.Id == id,
                Include = query => query
                    .Include(eventNews => eventNews.PreviewImage)
                    .Include(eventNews => eventNews.BackgroundImage)
                    .Include(eventNews => eventNews.Categories)
                    .ThenInclude(category => category.Localizations)
                    .ThenInclude(localization => localization.Language)
                    .Include(eventNews => eventNews.Localizations)
                    .ThenInclude(localization => localization.Language),
                AsNoTracking = false,
                AsSplitQuery = true
            });
    }

    private static IEnumerable<long> GetRequestedImageIds(UpdateEventNewsDto dto)
    {
        if (dto.PreviewImageId.HasValue)
        {
            yield return dto.PreviewImageId.Value;
        }

        if (dto.BackgroundImageId.HasValue)
        {
            yield return dto.BackgroundImageId.Value;
        }
    }

    private static bool HasScalarOrImageChanges(
        EventNewsEntity eventNews,
        UpdateEventNewsDto dto)
    {
        return !string.Equals(eventNews.Resource, dto.Resource, StringComparison.Ordinal)
               || eventNews.PublishedAt != dto.PublishedAt
               || eventNews.Status != dto.Status
               || eventNews.PreviewImageId != dto.PreviewImageId
               || eventNews.BackgroundImageId != dto.BackgroundImageId;
    }

    private static bool HaveSameCategoryIds(
        IEnumerable<EventNewsCategory> currentCategories,
        IEnumerable<long> requestedCategoryIds)
    {
        return currentCategories.Select(category => category.Id).ToHashSet()
            .SetEquals(requestedCategoryIds);
    }

    private static bool HaveLocalizationChanges(
        IEnumerable<EventNewsLocalization> currentLocalizations,
        IReadOnlyCollection<CreateEventNewsLocalizationDto> requestedLocalizations)
    {
        var currentByLanguageId = currentLocalizations.ToDictionary(localization => localization.LanguageId);
        if (currentByLanguageId.Count != requestedLocalizations.Count)
        {
            return true;
        }

        return requestedLocalizations.Any(dto =>
            !currentByLanguageId.TryGetValue(dto.LanguageId, out var current)
            || !string.Equals(current.Title, dto.Title?.Trim(), StringComparison.Ordinal)
            || !string.Equals(current.Description, NormalizeOptional(dto.Description), StringComparison.Ordinal)
            || current.TranslationStatus != TranslationStatus.Relevant);
    }

    private static bool HaveLocalizationTitlesChanged(
        IEnumerable<EventNewsLocalization> currentLocalizations,
        IReadOnlyCollection<CreateEventNewsLocalizationDto> requestedLocalizations)
    {
        var currentTitles = currentLocalizations.ToDictionary(
            localization => localization.LanguageId,
            localization => localization.Title);
        var requestedTitles = requestedLocalizations.ToDictionary(
            localization => localization.LanguageId,
            localization => localization.Title!.Trim());

        return currentTitles.Count != requestedTitles.Count
               || requestedTitles.Any(pair =>
                   !currentTitles.TryGetValue(pair.Key, out var title)
                   || !string.Equals(title, pair.Value, StringComparison.Ordinal));
    }

    private static void ApplyScalarAndImageChanges(
        EventNewsEntity eventNews,
        UpdateEventNewsDto dto,
        IReadOnlyDictionary<long, Image> imagesById)
    {
        eventNews.Resource = dto.Resource;
        eventNews.PublishedAt = dto.PublishedAt;
        eventNews.Status = dto.Status;
        eventNews.PreviewImageId = dto.PreviewImageId;
        eventNews.PreviewImage = dto.PreviewImageId.HasValue
            ? imagesById[dto.PreviewImageId.Value]
            : null;
        eventNews.BackgroundImageId = dto.BackgroundImageId;
        eventNews.BackgroundImage = dto.BackgroundImageId.HasValue
            ? imagesById[dto.BackgroundImageId.Value]
            : null;
    }

    private static void ReplaceCategories(
        EventNewsEntity eventNews,
        IEnumerable<EventNewsCategory> categories)
    {
        eventNews.Categories.Clear();
        foreach (var category in categories)
        {
            eventNews.Categories.Add(category);
        }
    }

    private static void MergeLocalizations(
        EventNewsEntity eventNews,
        IReadOnlyCollection<CreateEventNewsLocalizationDto> localizationDtos,
        IReadOnlyDictionary<long, LocalizationLanguage> languagesById)
    {
        var requestedByLanguageId = localizationDtos.ToDictionary(localization => localization.LanguageId);
        var localizationsToRemove = eventNews.Localizations
            .Where(localization => !requestedByLanguageId.ContainsKey(localization.LanguageId))
            .ToList();

        foreach (var localization in localizationsToRemove)
        {
            eventNews.Localizations.Remove(localization);
        }

        var currentByLanguageId = eventNews.Localizations.ToDictionary(localization => localization.LanguageId);
        var now = DateTimeOffset.UtcNow;

        foreach (var localizationDto in localizationDtos)
        {
            if (currentByLanguageId.TryGetValue(localizationDto.LanguageId, out var localization))
            {
                localization.Title = localizationDto.Title!.Trim();
                localization.Description = NormalizeOptional(localizationDto.Description);
                localization.TranslationStatus = TranslationStatus.Relevant;
                continue;
            }

            eventNews.Localizations.Add(new EventNewsLocalization
            {
                LanguageId = localizationDto.LanguageId,
                Language = languagesById[localizationDto.LanguageId],
                Title = localizationDto.Title!.Trim(),
                Description = NormalizeOptional(localizationDto.Description),
                TranslationStatus = TranslationStatus.Relevant,
                CreatedAt = now
            });
        }
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
