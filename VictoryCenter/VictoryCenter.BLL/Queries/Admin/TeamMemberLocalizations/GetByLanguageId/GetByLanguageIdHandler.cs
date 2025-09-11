using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.TeamMemberLocalizations;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.TeamMemberLocalizations.GetByLanguageId;

public class GetByLanguageIdHandler : IRequestHandler<GetByLanguageIdQuery, Result<IEnumerable<TeamMemberLocalizationDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repository;

    public GetByLanguageIdHandler(IMapper mapper, IRepositoryWrapper repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<Result<IEnumerable<TeamMemberLocalizationDto>>> Handle(GetByLanguageIdQuery request, CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<TeamMemberLocalization>
        {
            Filter = l => l.LanguageId == request.Id,
            Include = l => l.Include(loc => loc.Language),
        };
        IEnumerable<TeamMemberLocalization> localizations = await _repository.TeamMemberLocalizationsRepository.GetAllAsync(queryOptions);
        List<TeamMemberLocalizationDto>? localizationsDto = _mapper.Map<List<TeamMemberLocalizationDto>>(localizations);

        return Result.Ok(localizationsDto.AsEnumerable());
    }
}
