using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamCategories;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.Localization.TeamCategories.GetByLanguageId;
public class GetTeamCategoryLocalizationByLanguageIdHandler : IRequestHandler<GetTeamCategoryLocalizationByLanguageIdQuery, Result<List<TeamCategoryLocalizationDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetTeamCategoryLocalizationByLanguageIdHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<List<TeamCategoryLocalizationDto>>> Handle(GetTeamCategoryLocalizationByLanguageIdQuery request, CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<TeamCategoryLocalization>()
        {
            Filter = l => l.LanguageId == request.Id,
            Include = l => l.Include(loc => loc.Language),
            AsNoTracking = true,
        };

        IEnumerable<TeamCategoryLocalization>? entities = await _repositoryWrapper.TeamCategoryLocalizationsRepository.GetAllAsync(queryOptions);
        List<TeamCategoryLocalizationDto>? responseDto = _mapper.Map<List<TeamCategoryLocalizationDto>>(entities);
        return Result.Ok(responseDto);
    }
}
