using AutoMapper;
using FluentResults;
using MediatR;
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

namespace VictoryCenter.BLL.Commands.Admin.EventNews.Create;

public class CreateEventNewsHandler : IRequestHandler<CreateEventNewsCommand, Result<EventNewsDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ISlugService _slugService;
    private readonly IReorderService _reorderService;

    public CreateEventNewsHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        ISlugService slugService,
        IReorderService reorderService)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _slugService = slugService;
        _reorderService = reorderService;
    }

    public async Task<Result<EventNewsDto>> Handle(
        CreateEventNewsCommand request,
        CancellationToken cancellationToken)
    {
        var localizationDtos = request.CreateEventNewsDto.Localizations ?? [];

        var category = await _repositoryWrapper.EventNewsCategoryRepository.GetFirstOrDefaultAsync(
            new QueryOptions<EventNewsCategory>
            {
                Filter = item => item.Id == request.CreateEventNewsDto.CategoryId,
                AsNoTracking = false
            });

        if (category is null)
        {
            return Result.Fail<EventNewsDto>(ErrorMessagesConstants.NotFound(
                request.CreateEventNewsDto.CategoryId,
                typeof(EventNewsCategory)));
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
        eventNews.Category = category;
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

        await using var transaction = await _repositoryWrapper.BeginTransactionAsync(cancellationToken);

        try
        {
            eventNews.Priority = await _reorderService.GetNextDisplayOrderAsync<EventNewsEntity>(
                item => item.CategoryId == category.Id);

            await _repositoryWrapper.EventNewsRepository.CreateAsync(eventNews);

            if (await EventNewsAggregateHelper.SaveWithSlugRetryAsync(
                    _repositoryWrapper,
                    _slugService,
                    eventNews,
                    titleForSlug,
                    cancellationToken) > 0)
            {
                await transaction.CommitAsync(cancellationToken);

                return Result.Ok(_mapper.Map<EventNewsDto>(eventNews));
            }

            await transaction.RollbackAsync(cancellationToken);

            return Result.Fail<EventNewsDto>(ErrorMessagesConstants.FailedToCreateEntity(typeof(EventNewsEntity)));
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
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
                AdditionalDescription = localizationDto.AdditionalDescription?.Trim(),
                TranslationStatus = TranslationStatus.Relevant,
                CreatedAt = createdAt
            };

            eventNews.Localizations.Add(localization);
        }

        return Result.Ok();
    }
}
