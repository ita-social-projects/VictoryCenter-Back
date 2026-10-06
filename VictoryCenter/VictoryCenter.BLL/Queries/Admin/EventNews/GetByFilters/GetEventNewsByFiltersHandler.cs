using System.Linq.Expressions;
using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.EventNews;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using EventNewsCategoryLink = VictoryCenter.DAL.Entities.EventNewsEventNewsCategories;
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

        Expression<Func<EventNewsEntity, bool>> filter = eventNews =>
            (!categoryId.HasValue ||
                eventNews.Categories.Any(category => category.Id == categoryId.Value)) &&
            (!status.HasValue ||
                eventNews.Status == status.Value);

        var prioritiesFilter = new QueryOptions<EventNewsCategoryLink>
        {
            Filter = priority => !categoryId.HasValue || priority.CategoriesId == categoryId.Value,
            AsNoTracking = true
        };

        var allPriorities = await _repositoryWrapper
            .EventNewsEventNewsCategoriesRepository
            .GetAllAsync(prioritiesFilter);

        var totalCount = await _repositoryWrapper.EventNewsRepository.CountAsync(
            new QueryOptions<EventNewsEntity>
            {
                Filter = filter,
                AsNoTracking = true
            });

        var offset = request.Filter.Offset ?? 0;
        var limit = request.Filter.Limit ?? DefaultLimit;

        var eventNewsIds = allPriorities
            .OrderBy(p => p.Priority)
            .Select(p => p.EventsNewsId)
            .Distinct()
            .Skip(offset)
            .Take(limit)
            .ToList();

        var queryOptions = SetupEventNewsQueryOptions(eventNewsIds);

        var eventNews = await _repositoryWrapper.EventNewsRepository.GetAllAsync(queryOptions);

        var items = _mapper.Map<EventNewsDto[]>(eventNews);

        var sortedItems = items
            .OrderBy(item => allPriorities
                .FirstOrDefault(p => p.EventsNewsId == item.Id && p.CategoriesId == (categoryId ?? 0L))
                ?.Priority ?? 0)
            .ToArray();

        return Result.Ok(new PaginationResult<EventNewsDto>(sortedItems, totalCount));
        }

    private static QueryOptions<EventNewsEntity> SetupEventNewsQueryOptions(List<long> eventNewsIds)
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
