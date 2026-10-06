using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using EventNewsEntity = VictoryCenter.DAL.Entities.EventNews;

namespace VictoryCenter.BLL.Queries.Public.EventNews.GetPublishedBySlug;

public class GetPublishedEventNewsBySlugHandler
    : IRequestHandler<GetPublishedEventNewsBySlugQuery, Result<PublishedEventNewsDetailsDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetPublishedEventNewsBySlugHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<PublishedEventNewsDetailsDto>> Handle(
        GetPublishedEventNewsBySlugQuery request,
        CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<EventNewsEntity>
        {
            Filter = eventNews =>
                eventNews.Slug == request.Slug &&
                eventNews.Status == Status.Published,
            Include = eventNews => eventNews
                .Include(e => e.Categories)
                    .ThenInclude(category => category.Localizations)
                        .ThenInclude(localization => localization.Language)
                .Include(e => e.PreviewImage)
                .Include(e => e.BackgroundImage)
                .Include(e => e.Localizations)
                    .ThenInclude(l => l.Language),
            AsSplitQuery = true,
            AsNoTracking = true,
        };

        var eventNews = await _repositoryWrapper.EventNewsRepository.GetFirstOrDefaultAsync(queryOptions);

        if (eventNews is null)
        {
            return Result.Fail<PublishedEventNewsDetailsDto>(
                $"Published event news with slug '{request.Slug}' was not found");
        }

        return Result.Ok(_mapper.Map<PublishedEventNewsDetailsDto>(eventNews));
    }
}
