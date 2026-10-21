using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.EventsPage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.EventsPage.Get;

public class GetEventsIntroSectionHandler : IRequestHandler<GetEventsIntroSectionQuery, Result<EventsIntroSectionDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;

    public GetEventsIntroSectionHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
    }

    public async Task<Result<EventsIntroSectionDto>> Handle(GetEventsIntroSectionQuery request, CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<EventsIntroSection>
        {
            Include = q => q.Include(e => e.Localizations).ThenInclude(l => l.Language),
            AsNoTracking = true,
        };

        var entity = await _repositoryWrapper.EventsIntroSectionsRepository.GetFirstOrDefaultAsync(queryOptions);
        return entity == null
            ? Result.Fail<EventsIntroSectionDto>(ErrorMessagesConstants.NotFound())
            : Result.Ok(_mapper.Map<EventsIntroSectionDto>(entity));
    }
}
