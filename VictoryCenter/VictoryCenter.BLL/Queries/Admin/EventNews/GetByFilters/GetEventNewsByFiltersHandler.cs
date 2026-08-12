using System.Linq.Expressions;
using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.EventNews;
using VictoryCenter.BLL.DTOs.Common;
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
        Expression<Func<EventNewsEntity, bool>> filter = eventNews =>
            !categoryId.HasValue || eventNews.Categories.Any(category => category.Id == categoryId.Value);

        var queryOptions = new QueryOptions<EventNewsEntity>
        {
            Filter = filter,
            Include = eventNews => eventNews
                .AsSplitQuery()
                .Include(entity => entity.PreviewImage)
                .Include(entity => entity.BackgroundImage)
                .Include(entity => entity.Categories)
                    .ThenInclude(category => category.Localizations)
                        .ThenInclude(localization => localization.Language)
                .Include(entity => entity.Localizations)
                    .ThenInclude(localization => localization.Language),
            Offset = request.Filter.Offset ?? 0,
            Limit = request.Filter.Limit ?? DefaultLimit,
            OrderByDESC = eventNews => eventNews.Id,
            AsNoTracking = true
        };

        var eventNews = await _repositoryWrapper.EventNewsRepository.GetAllAsync(queryOptions);
        var totalCount = await _repositoryWrapper.EventNewsRepository.CountAsync(
            new QueryOptions<EventNewsEntity>
            {
                Filter = filter,
                AsNoTracking = true
            });

        var items = _mapper.Map<EventNewsDto[]>(eventNews);

        return Result.Ok(new PaginationResult<EventNewsDto>(items, totalCount));
    }
}
