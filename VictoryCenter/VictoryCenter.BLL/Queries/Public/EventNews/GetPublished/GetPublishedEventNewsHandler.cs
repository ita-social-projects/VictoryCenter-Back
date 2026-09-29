using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Public.EventNews;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using EventNewsCategoryLink = VictoryCenter.DAL.Entities.EventNewsEventNewsCategories;
using EventNewsEntity = VictoryCenter.DAL.Entities.EventNews;

namespace VictoryCenter.BLL.Queries.Public.EventNews.GetPublished;

public class GetPublishedEventNewsHandler
    : IRequestHandler<GetPublishedEventNewsQuery, Result<List<PublishedEventNewsDto>>>
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

    public async Task<Result<List<PublishedEventNewsDto>>> Handle(
        GetPublishedEventNewsQuery request,
        CancellationToken cancellationToken)
    {
        var publishedEventNews = await _repositoryWrapper.EventNewsRepository
            .GetAllAsync(new QueryOptions<EventNewsEntity>
            {
                Filter = eventNews => eventNews.Status == Status.Published,
                Include = eventNews => eventNews
                    .Include(e => e.Categories)
                    .Include(e => e.PreviewImage)
                    .Include(e => e.Localizations)
                        .ThenInclude(localization => localization.Language),
                AsNoTracking = true
            });

        var publishedEventNewsIds = publishedEventNews
            .Select(eventNews => eventNews.Id)
            .ToList();

        var priorities = await _repositoryWrapper
            .EventNewsEventNewsCategoriesRepository
            .GetAllAsync(new QueryOptions<EventNewsCategoryLink>
            {
                Filter = link => publishedEventNewsIds.Contains(link.EventsNewsId),
                AsNoTracking = true
            });

        var priorityByEventNewsId = priorities
            .GroupBy(link => link.EventsNewsId)
            .ToDictionary(
                group => group.Key,
                group => group.Min(link => link.Priority));

        var sortedEventNews = publishedEventNews
            .OrderBy(eventNews => priorityByEventNewsId.ContainsKey(eventNews.Id)
                ? priorityByEventNewsId[eventNews.Id]
                : long.MaxValue)
            .ThenByDescending(eventNews => eventNews.PublishedAt)
            .Take(request.Take ?? publishedEventNews.Count())
            .ToList();

        var result = _mapper
            .Map<List<PublishedEventNewsDto>>(sortedEventNews);

        return Result.Ok(result);
    }
}
