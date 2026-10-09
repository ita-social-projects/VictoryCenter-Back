using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.EventNews;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.BLL.Interfaces.ReorderService;
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
    private readonly IReorderService _reorderService;
    private readonly TimeProvider _timeProvider;

    public UpdateEventNewsHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        ISlugService slugService,
        IReorderService reorderService,
        TimeProvider timeProvider)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _slugService = slugService;
        _reorderService = reorderService;
        _timeProvider = timeProvider;
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
        var localizationDtos = (dto.Localizations ?? [])
            .Where(localization => localization is not null && !string.IsNullOrWhiteSpace(localization.Title))
            .ToList();

        var category = await _repositoryWrapper.EventNewsCategoryRepository.GetFirstOrDefaultAsync(
            new QueryOptions<EventNewsCategory>
            {
                Filter = item => item.Id == dto.CategoryId,
                Include = query => query
                    .Include(item => item.Localizations)
                    .ThenInclude(localization => localization.Language),
                AsNoTracking = false
            });

        if (category is null)
        {
            return Result.Fail<EventNewsDto>(
                ErrorMessagesConstants.NotFound(dto.CategoryId, typeof(EventNewsCategory)));
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

        var (categoryChanged, localizationsChanged, titlesChanged, sourceContentChanged, hasChanges) = GetChanges(
            eventNews,
            dto,
            localizationDtos);

        var oldCategoryId = eventNews.CategoryId;

        if (!hasChanges)
        {
            return Result.Ok(_mapper.Map<EventNewsDto>(eventNews));
        }

        await using var transaction = await _repositoryWrapper.BeginTransactionAsync(cancellationToken);

        try
        {
            await ApplyChangesAsync(
                eventNews,
                dto,
                imagesResult.Value,
                category,
                localizationDtos,
                languagesResult.Value,
                categoryChanged,
                localizationsChanged,
                sourceContentChanged);

            var titleForSlug = localizationDtos
                .Select(localization => localization.Title?.Trim())
                .FirstOrDefault(title => !string.IsNullOrWhiteSpace(title));

            await UpdateSlugAsync(eventNews, titleForSlug, titlesChanged, cancellationToken);

            var result = await SaveAsync(eventNews, titleForSlug, cancellationToken);

            if (result.IsFailed)
            {
                await transaction.RollbackAsync(cancellationToken);

                return result;
            }

            if (categoryChanged)
            {
                await _reorderService.RenumberPriorityAsync<EventNewsEntity>(
                    item => item.CategoryId == oldCategoryId);
            }

            await transaction.CommitAsync(cancellationToken);

            return result;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
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
                    .Include(eventNews => eventNews.Category)
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
        var scalarFieldsChanged = !string.Equals(eventNews.Title, dto.Title)
                                  || !string.Equals(eventNews.Description, dto.Description)
                                  || !string.Equals(eventNews.AdditionalDescription, dto.AdditionalDescription, StringComparison.Ordinal)
                                  || !string.Equals(eventNews.Resource, dto.Resource, StringComparison.Ordinal)
                                  || !string.Equals(eventNews.ResourceEn, dto.ResourceEn, StringComparison.Ordinal)
                                  || eventNews.PublishedAt != dto.PublishedAt
                                  || eventNews.Status != dto.Status;
        var imagesChanged = eventNews.PreviewImageId != dto.PreviewImageId
                            || eventNews.BackgroundImageId != dto.BackgroundImageId;

        return scalarFieldsChanged || imagesChanged;
    }

    private static bool HasSourceContentChanges(
        EventNewsEntity eventNews,
        UpdateEventNewsDto dto)
    {
        return !string.Equals(eventNews.Title, dto.Title, StringComparison.Ordinal)
               || !string.Equals(eventNews.Description, dto.Description, StringComparison.Ordinal)
               || !string.Equals(
                   eventNews.AdditionalDescription,
                   dto.AdditionalDescription,
                   StringComparison.Ordinal);
    }

    private static (bool LocalizationsChanged, bool TitlesChanged) GetLocalizationChanges(
        IEnumerable<EventNewsLocalization> currentLocalizations,
        IReadOnlyCollection<CreateEventNewsLocalizationDto> requestedLocalizations)
    {
        var currentByLanguageId = currentLocalizations.ToDictionary(localization => localization.LanguageId);
        var localizationsChanged = currentByLanguageId.Count != requestedLocalizations.Count;
        var titlesChanged = localizationsChanged;

        foreach (var dto in requestedLocalizations)
        {
            if (!currentByLanguageId.TryGetValue(dto.LanguageId, out var current))
            {
                localizationsChanged = true;
                titlesChanged = true;
                continue;
            }

            var titleChanged = !string.Equals(current.Title, dto.Title?.Trim(), StringComparison.Ordinal);
            titlesChanged |= titleChanged;
            localizationsChanged |= titleChanged
                                    || !string.Equals(
                                        current.Description,
                                        NormalizeOptional(dto.Description),
                                        StringComparison.Ordinal)
                                    || !string.Equals(
                                        current.AdditionalDescription,
                                        NormalizeOptional(dto.AdditionalDescription),
                                        StringComparison.Ordinal);
        }

        return (localizationsChanged, titlesChanged);
    }

    private static void ApplyScalarAndImageChanges(
        EventNewsEntity eventNews,
        UpdateEventNewsDto dto,
        IReadOnlyDictionary<long, Image> imagesById)
    {
        eventNews.Title = dto.Title;
        eventNews.Description = dto.Description;
        eventNews.AdditionalDescription = dto.AdditionalDescription;
        eventNews.Resource = dto.Resource;
        eventNews.ResourceEn = dto.ResourceEn;
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

    private void MergeLocalizations(
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
        var now = _timeProvider.GetUtcNow();

        foreach (var localizationDto in localizationDtos)
        {
            if (currentByLanguageId.TryGetValue(localizationDto.LanguageId, out var localization))
            {
                var title = localizationDto.Title!.Trim();
                var description = NormalizeOptional(localizationDto.Description);
                var additionalDescription = NormalizeOptional(localizationDto.AdditionalDescription);
                if (string.Equals(localization.Title, title, StringComparison.Ordinal)
                    && string.Equals(localization.Description, description, StringComparison.Ordinal)
                    && string.Equals(localization.AdditionalDescription, additionalDescription, StringComparison.Ordinal))
                {
                    continue;
                }

                localization.Title = title;
                localization.Description = description;
                localization.AdditionalDescription = additionalDescription;
                localization.TranslationStatus = TranslationStatus.Relevant;
                continue;
            }

            eventNews.Localizations.Add(new EventNewsLocalization
            {
                LanguageId = localizationDto.LanguageId,
                Language = languagesById[localizationDto.LanguageId],
                Title = localizationDto.Title!.Trim(),
                Description = NormalizeOptional(localizationDto.Description),
                AdditionalDescription = NormalizeOptional(localizationDto.AdditionalDescription),
                TranslationStatus = TranslationStatus.Relevant,
                CreatedAt = now
            });
        }
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static (
        bool CategoryChanged,
        bool LocalizationsChanged,
        bool TitlesChanged,
        bool SourceContentChanged,
        bool HasChanges) GetChanges(
        EventNewsEntity eventNews,
        UpdateEventNewsDto dto,
        IReadOnlyCollection<CreateEventNewsLocalizationDto> localizationDtos)
    {
        var categoryChanged = eventNews.CategoryId != dto.CategoryId;
        var (localizationsChanged, titlesChanged) = GetLocalizationChanges(
            eventNews.Localizations,
            localizationDtos);
        var sourceContentChanged = HasSourceContentChanges(eventNews, dto);
        var slugStateChanged = string.IsNullOrWhiteSpace(eventNews.Slug) != (localizationDtos.Count == 0);
        var hasChanges = HasScalarOrImageChanges(eventNews, dto)
                         || categoryChanged
                         || localizationsChanged
                         || slugStateChanged;

        return (categoryChanged, localizationsChanged, titlesChanged, sourceContentChanged, hasChanges);
    }

    private async Task ApplyChangesAsync(
        EventNewsEntity eventNews,
        UpdateEventNewsDto dto,
        IReadOnlyDictionary<long, Image> imagesById,
        EventNewsCategory category,
        IReadOnlyCollection<CreateEventNewsLocalizationDto> localizationDtos,
        IReadOnlyDictionary<long, LocalizationLanguage> languagesById,
        bool categoryChanged,
        bool localizationsChanged,
        bool sourceContentChanged)
    {
        if (sourceContentChanged)
        {
            foreach (var localization in eventNews.Localizations)
            {
                localization.TranslationStatus = TranslationStatus.Outdated;
            }
        }

        ApplyScalarAndImageChanges(eventNews, dto, imagesById);

        if (categoryChanged)
        {
            eventNews.CategoryId = category.Id;
            eventNews.Category = category;
            eventNews.Priority = await _reorderService
                .GetNextDisplayOrderAsync<EventNewsEntity>(item => item.CategoryId == category.Id);
        }

        if (localizationsChanged)
        {
            MergeLocalizations(eventNews, localizationDtos, languagesById);
        }
    }

    private async Task UpdateSlugAsync(
        EventNewsEntity eventNews,
        string? titleForSlug,
        bool titlesChanged,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(titleForSlug))
        {
            eventNews.Slug = null;
            return;
        }

        if (!titlesChanged && !string.IsNullOrWhiteSpace(eventNews.Slug))
        {
            return;
        }

        eventNews.Slug = await _slugService.GenerateUniqueEventNewsSlugAsync(
            eventNews.Id,
            titleForSlug,
            cancellationToken);
    }

    private async Task<Result<EventNewsDto>> SaveAsync(
        EventNewsEntity eventNews,
        string? titleForSlug,
        CancellationToken cancellationToken)
    {
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
            if (!await _repositoryWrapper.EventNewsRepository.ExistsAsync(
                    entity => entity.Id == eventNews.Id))
            {
                return Result.Fail<EventNewsDto>(
                    ErrorMessagesConstants.NotFound(eventNews.Id, typeof(EventNewsEntity)));
            }

            throw;
        }

        return Result.Fail<EventNewsDto>(
            ErrorMessagesConstants.FailedToUpdateEntity(typeof(EventNewsEntity)));
    }
}
