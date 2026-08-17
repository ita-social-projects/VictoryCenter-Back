using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.EventNews;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using EventNewsEntity = VictoryCenter.DAL.Entities.EventNews;

namespace VictoryCenter.BLL.Queries.Admin.EventNews.GetById;

public class GetEventNewsByIdHandler : IRequestHandler<GetEventNewsByIdQuery, Result<EventNewsDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetEventNewsByIdHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<EventNewsDto>> Handle(
        GetEventNewsByIdQuery request,
        CancellationToken cancellationToken)
    {
        var eventNews = await _repositoryWrapper.EventNewsRepository.GetFirstOrDefaultAsync(
            new QueryOptions<EventNewsEntity>
            {
                Filter = entity => entity.Id == request.Id,
                Include = entity => entity
                    .AsSplitQuery()
                    .Include(item => item.PreviewImage)
                    .Include(item => item.BackgroundImage)
                    .Include(item => item.Categories)
                        .ThenInclude(category => category.Localizations)
                            .ThenInclude(localization => localization.Language)
                    .Include(item => item.Localizations)
                        .ThenInclude(localization => localization.Language),
                AsNoTracking = true
            });

        return eventNews is null
            ? Result.Fail<EventNewsDto>(ErrorMessagesConstants.NotFound(request.Id, typeof(EventNewsEntity)))
            : Result.Ok(_mapper.Map<EventNewsDto>(eventNews));
    }
}
