using System.Linq.Expressions;
using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.EventNews;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Enums;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using EventNewsEntity = VictoryCenter.DAL.Entities.EventNews;

namespace VictoryCenter.BLL.Queries.Admin.EventNews.GetByFilters;

public class GetEventNewsByFiltersHandler
    : IRequestHandler<GetEventNewsByFiltersQuery, Result<PaginationResult<EventNewsDto>>>
{
    private const int DefaultLimit = 20;

    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetEventNewsByFiltersHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<PaginationResult<EventNewsDto>>> Handle(
        GetEventNewsByFiltersQuery request,
        CancellationToken cancellationToken)
    {
        var categoryId = request.Filter.CategoryId;
        var status = request.Filter.Status;
        var translationStatusFilter = request.Filter.TranslationStatusFilter;
        var requiredLocalizationCount = translationStatusFilter == TranslationStatusFilter.Missing
            ? Math.Max(
                0,
                await _repositoryWrapper.LocalizationLanguagesRepository.CountAsync() - 1)
            : 0;

        Expression<Func<EventNewsEntity, bool>> filter = eventNews =>
            (!categoryId.HasValue ||
                eventNews.Categories.Any(category => category.Id == categoryId.Value)) &&
            (!status.HasValue ||
                eventNews.Status == status.Value) &&
            (!translationStatusFilter.HasValue ||
                translationStatusFilter == TranslationStatusFilter.All ||
                (translationStatusFilter == TranslationStatusFilter.Outdated &&
                    eventNews.Localizations.Any(localization =>
                        localization.TranslationStatus == TranslationStatus.Outdated)) ||
                (translationStatusFilter == TranslationStatusFilter.Missing &&
                    eventNews.Localizations.Count(localization =>
                        localization.LanguageId != LocalizationLanguageConstants.PrimaryLanguageId) <
                    requiredLocalizationCount));

        var totalCount = await _repositoryWrapper.EventNewsRepository.CountAsync(
            new QueryOptions<EventNewsEntity>
            {
                Filter = filter,
                AsNoTracking = true
            });

        var offset = request.Filter.Offset ?? 0;
        var limit = request.Filter.Limit ?? DefaultLimit;

        var eventNewsIds = await _repositoryWrapper.EventNewsRepository.GetPagedIdsByFilterAsync(
            filter,
            categoryId,
            offset,
            limit,
            cancellationToken);

        var queryOptions = SetupEventNewsQueryOptions(eventNewsIds);

        var eventNews = await _repositoryWrapper.EventNewsRepository.GetAllAsync(queryOptions);

        var items = _mapper.Map<EventNewsDto[]>(eventNews);

        var itemOrder = eventNewsIds
            .Select((id, index) => new { id, index })
            .ToDictionary(item => item.id, item => item.index);

        var sortedItems = items
            .OrderBy(item => itemOrder[item.Id])
            .ToArray();

        return Result.Ok(new PaginationResult<EventNewsDto>(sortedItems, totalCount));
    }

    private static QueryOptions<EventNewsEntity> SetupEventNewsQueryOptions(
        IReadOnlyCollection<long> eventNewsIds)
    {
        return new QueryOptions<EventNewsEntity>
        {
            Filter = eventNews => eventNewsIds.Contains(eventNews.Id),
            Include = eventNews => eventNews
                        .AsSplitQuery()
                        .Include(entity => entity.PreviewImage)
                        .Include(entity => entity.BackgroundImage)
                        .Include(entity => entity.Categories)
                            .ThenInclude(category => category.Localizations)
                                .ThenInclude(localization => localization.Language)
                        .Include(entity => entity.Localizations)
                            .ThenInclude(localization => localization.Language),
            AsNoTracking = true
        };
    }
}
