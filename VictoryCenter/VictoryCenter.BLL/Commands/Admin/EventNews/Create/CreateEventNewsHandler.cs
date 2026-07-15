using AutoMapper;
using FluentResults;
using MediatR;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.EventNews;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.BLL.Interfaces.SlugService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using EventNewsEntity = VictoryCenter.DAL.Entities.EventNews;

namespace VictoryCenter.BLL.Commands.Admin.EventNews.Create;

public class CreateEventNewsHandler : IRequestHandler<CreateEventNewsCommand, Result<EventNewsDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ISlugService _slugService;

    public CreateEventNewsHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        ISlugService slugService)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _slugService = slugService;
    }

    public async Task<Result<EventNewsDto>> Handle(
        CreateEventNewsCommand request,
        CancellationToken cancellationToken)
    {
        var categoriesResult = await CategoryValidationHelper.ValidateAndGetEventNewsCategoriesAsync(
            _repositoryWrapper,
            request.CreateEventNewsDto.CategoryIds);

        if (categoriesResult.IsFailed)
        {
            return Result.Fail<EventNewsDto>(categoriesResult.Errors);
        }

        var previewImageResult = await ImageValidationHelper.ValidateAndGetImageAsync(
            _repositoryWrapper,
            request.CreateEventNewsDto.PreviewImageId);

        if (previewImageResult.IsFailed)
        {
            return Result.Fail<EventNewsDto>(previewImageResult.Errors);
        }

        var backgroundImageResult = await ImageValidationHelper.ValidateAndGetImageAsync(
            _repositoryWrapper,
            request.CreateEventNewsDto.BackgroundImageId);

        if (backgroundImageResult.IsFailed)
        {
            return Result.Fail<EventNewsDto>(backgroundImageResult.Errors);
        }

        var languagesResult = await ValidateAndGetLanguagesAsync(request.CreateEventNewsDto.Localizations);

        if (languagesResult.IsFailed)
        {
            return Result.Fail<EventNewsDto>(languagesResult.Errors);
        }

        var eventNews = _mapper.Map<EventNewsEntity>(request.CreateEventNewsDto);
        var now = DateTimeOffset.UtcNow;
        eventNews.CreatedAt = now;
        eventNews.PreviewImage = previewImageResult.Value;
        eventNews.BackgroundImage = backgroundImageResult.Value;

        AddCategories(eventNews, categoriesResult.Value);
        AddLocalizations(eventNews, request.CreateEventNewsDto.Localizations, languagesResult.Value, now);

        var titleForSlug = eventNews.Localizations
            .Select(localization => localization.Title)
            .FirstOrDefault(title => !string.IsNullOrWhiteSpace(title));

        if (!string.IsNullOrWhiteSpace(titleForSlug))
        {
            eventNews.Slug = await _slugService.GenerateUniqueEventNewsSlugAsync(
                eventNews.Id,
                titleForSlug,
                cancellationToken);
        }

        await _repositoryWrapper.EventNewsRepository.CreateAsync(eventNews);

        if (await _repositoryWrapper.SaveChangesAsync() > 0)
        {
            return Result.Ok(_mapper.Map<EventNewsDto>(eventNews));
        }

        return Result.Fail<EventNewsDto>(ErrorMessagesConstants.FailedToCreateEntity(typeof(EventNewsEntity)));
    }

    private async Task<Result<IReadOnlyDictionary<long, LocalizationLanguage>>> ValidateAndGetLanguagesAsync(
        IEnumerable<CreateEventNewsLocalizationDto> localizations)
    {
        var languageIds = localizations
            .Select(localization => localization.LanguageId)
            .Where(id => id > 0)
            .Distinct()
            .ToList();

        if (languageIds.Count == 0)
        {
            return Result.Ok<IReadOnlyDictionary<long, LocalizationLanguage>>(
                new Dictionary<long, LocalizationLanguage>());
        }

        var languages = (await _repositoryWrapper.LocalizationLanguagesRepository.GetAllAsync(
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

    private static void AddCategories(EventNewsEntity eventNews, ICollection<EventNewsCategory> categories)
    {
        foreach (var category in categories)
        {
            eventNews.Categories.Add(category);
        }
    }

    private static void AddLocalizations(
        EventNewsEntity eventNews,
        IEnumerable<CreateEventNewsLocalizationDto> localizationDtos,
        IReadOnlyDictionary<long, LocalizationLanguage> languagesById,
        DateTimeOffset createdAt)
    {
        foreach (var localizationDto in localizationDtos)
        {
            if (string.IsNullOrWhiteSpace(localizationDto.Title))
            {
                continue;
            }

            var localization = new EventNewsLocalization
            {
                LanguageId = localizationDto.LanguageId,
                Language = languagesById[localizationDto.LanguageId],
                Title = localizationDto.Title!.Trim(),
                Description = localizationDto.Description?.Trim(),
                CreatedAt = createdAt
            };

            eventNews.Localizations.Add(localization);
        }
    }
}
