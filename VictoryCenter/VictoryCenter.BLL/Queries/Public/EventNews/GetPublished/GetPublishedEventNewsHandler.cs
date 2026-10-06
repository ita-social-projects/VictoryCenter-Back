using System.Linq.Expressions;
using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.DTOs.Public.EventNews;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using EventNewsEntity = VictoryCenter.DAL.Entities.EventNews;

namespace VictoryCenter.BLL.Queries.Public.EventNews.GetPublished;

public class GetPublishedEventNewsHandler
    : IRequestHandler<GetPublishedEventNewsQuery, Result<PaginationResult<PublishedEventNewsDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetPublishedEventNewsHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<PaginationResult<PublishedEventNewsDto>>> Handle(
        GetPublishedEventNewsQuery request,
        CancellationToken cancellationToken)
    {
        var categoryId = request.CategoryId;
        var offset = Math.Max(request.Offset ?? 0, 0);
        var limit = Math.Clamp(
    request.Limit ?? EventNewsConstants.DefaultLimit,
    EventNewsConstants.PublishedTakeMinValue,
    EventNewsConstants.PublishedTakeMaxValue);

        Expression<Func<EventNewsEntity, bool>> filter = eventNews =>
            eventNews.Status == Status.Published &&
            (!categoryId.HasValue ||
                eventNews.Categories.Any(category => category.Id == categoryId.Value));

        var totalCount = await _repositoryWrapper.EventNewsRepository.CountAsync(
            new QueryOptions<EventNewsEntity>
            {
                Filter = filter,
                AsNoTracking = true,
            });

        if (totalCount == 0 || offset >= totalCount)
        {
            return Result.Ok(new PaginationResult<PublishedEventNewsDto>([], totalCount));
        }

        var queryOptions = new QueryOptions<EventNewsEntity>
        {
            Filter = filter,
            Include = eventNews => eventNews
                .Include(e => e.Categories)
                    .ThenInclude(category => category.Localizations)
                        .ThenInclude(localization => localization.Language)
                .Include(e => e.PreviewImage)
                .Include(e => e.Localizations)
                    .ThenInclude(l => l.Language),
            OrderByDESC = eventNews => eventNews.PublishedAt,
            ThenByDESC = eventNews => eventNews.Id,
            Offset = offset,
            Limit = limit,
            AsSplitQuery = true,
            AsNoTracking = true,
        };

        var publishedEventNews = await _repositoryWrapper.EventNewsRepository.GetAllAsync(queryOptions);

        var items = _mapper.Map<PublishedEventNewsDto[]>(publishedEventNews);

        return Result.Ok(new PaginationResult<PublishedEventNewsDto>(items, totalCount));
    }
}
