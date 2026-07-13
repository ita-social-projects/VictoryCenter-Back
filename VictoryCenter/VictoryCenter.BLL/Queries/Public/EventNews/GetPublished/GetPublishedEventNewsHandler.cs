using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Public.EventNews;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using EventNewsEntity = VictoryCenter.DAL.Entities.EventNews;

namespace VictoryCenter.BLL.Queries.Public.EventNews.GetPublished;

public class GetPublishedEventNewsHandler
    : IRequestHandler<GetPublishedEventNewsQuery, Result<List<PublishedEventNewsDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetPublishedEventNewsHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<List<PublishedEventNewsDto>>> Handle(
        GetPublishedEventNewsQuery request,
        CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<EventNewsEntity>
        {
            Filter = eventNews => eventNews.Status == Status.Published,
            Include = eventNews => eventNews
                .Include(e => e.Categories)
                .Include(e => e.PreviewImage)
                .Include(e => e.Localizations)
                    .ThenInclude(l => l.Language),
            OrderByDESC = eventNews => eventNews.PublishedAt,
            Limit = request.Take ?? 0,
        };

        IEnumerable<EventNewsEntity> publishedEventNews =
            await _repositoryWrapper.EventNewsRepository.GetAllAsync(queryOptions);

        var result = _mapper.Map<IEnumerable<PublishedEventNewsDto>>(publishedEventNews).ToList();

        return Result.Ok(result);
    }
}
