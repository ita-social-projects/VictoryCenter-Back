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
        var categoryIds = request.CreateEventNewsDto.CategoryIds ?? [];
        var localizationDtos = request.CreateEventNewsDto.Localizations ?? [];

        var categoriesResult = await CategoryValidationHelper.ValidateAndGetCategoriesAsync(
            _repositoryWrapper.EventNewsCategoryRepository,
            categoryIds);

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

        var localizationsToCreate = localizationDtos
            .Where(localization => localization is not null && !string.IsNullOrWhiteSpace(localization.Title))
            .ToList();

        var languagesResult = await EventNewsAggregateHelper.ValidateAndGetLanguagesAsync(
            _repositoryWrapper,
            localizationsToCreate);

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
        var localizationsResult = AddLocalizations(
            eventNews,
            localizationsToCreate,
            languagesResult.Value,
            now);

        if (localizationsResult.IsFailed)
        {
            return Result.Fail<EventNewsDto>(localizationsResult.Errors);
        }

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

        if (await EventNewsAggregateHelper.SaveWithSlugRetryAsync(
                _repositoryWrapper,
                _slugService,
                eventNews,
                titleForSlug,
                cancellationToken) > 0)
        {
            return Result.Ok(_mapper.Map<EventNewsDto>(eventNews));
        }

        return Result.Fail<EventNewsDto>(ErrorMessagesConstants.FailedToCreateEntity(typeof(EventNewsEntity)));
    }

    private static void AddCategories(EventNewsEntity eventNews, ICollection<EventNewsCategory> categories)
    {
        foreach (var category in categories)
        {
            eventNews.Categories.Add(category);
        }
    }

    private static Result AddLocalizations(
        EventNewsEntity eventNews,
        IEnumerable<CreateEventNewsLocalizationDto> localizationDtos,
        IReadOnlyDictionary<long, LocalizationLanguage> languagesById,
        DateTimeOffset createdAt)
    {
        foreach (var localizationDto in localizationDtos)
        {
            if (!languagesById.TryGetValue(localizationDto.LanguageId, out var language))
            {
                return Result.Fail(
                    ErrorMessagesConstants.NotFound(localizationDto.LanguageId, typeof(LocalizationLanguage)));
            }

            var localization = new EventNewsLocalization
            {
                LanguageId = localizationDto.LanguageId,
                Language = language,
                Title = localizationDto.Title!.Trim(),
                Description = localizationDto.Description?.Trim(),
                CreatedAt = createdAt
            };

            eventNews.Localizations.Add(localization);
        }

        return Result.Ok();
    }
}
