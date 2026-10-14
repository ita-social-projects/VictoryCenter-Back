using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventsIntroSection;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.Localization.EventsIntroSection.GetByEntityId;

public class GetEventsIntroSectionLocalizationByEntityIdHandler
    : IRequestHandler<GetEventsIntroSectionLocalizationByEntityIdQuery, Result<List<EventsIntroSectionLocalizationDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetEventsIntroSectionLocalizationByEntityIdHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<List<EventsIntroSectionLocalizationDto>>> Handle(
        GetEventsIntroSectionLocalizationByEntityIdQuery request,
        CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<EventsIntroSectionLocalization>
        {
            Filter = l => l.EntityId == request.Id,
            Include = l => l.Include(loc => loc.Language),
            AsNoTracking = true,
        };

        var entities = await _repositoryWrapper.EventsIntroSectionLocalizationsRepository.GetAllAsync(queryOptions);
        var responseDto = _mapper.Map<List<EventsIntroSectionLocalizationDto>>(entities);
        return Result.Ok(responseDto);
    }
}
