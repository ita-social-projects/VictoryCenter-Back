using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamMembers;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.Localization.TeamMembers.GetByEntityId;

public class GetTeamMemberLocalizationByEntityIdHandler : IRequestHandler<GetTeamMemberLocalizationByEntityIdQuery, Result<IEnumerable<TeamMemberLocalizationDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repository;

    public GetTeamMemberLocalizationByEntityIdHandler(IMapper mapper, IRepositoryWrapper repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<Result<IEnumerable<TeamMemberLocalizationDto>>> Handle(GetTeamMemberLocalizationByEntityIdQuery request, CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<TeamMemberLocalization>
        {
            Filter = l => l.EntityId == request.Id,
            Include = l => l.Include(loc => loc.Language),
            AsNoTracking = true,
        };
        IEnumerable<TeamMemberLocalization> localizations = await _repository.TeamMemberLocalizationsRepository.GetAllAsync(queryOptions);
        List<TeamMemberLocalizationDto>? localizationsDto = _mapper.Map<List<TeamMemberLocalizationDto>>(localizations);

        return Result.Ok(localizationsDto.AsEnumerable());
    }
}
