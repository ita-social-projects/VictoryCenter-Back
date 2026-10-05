using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageHippoventionCenterSection;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.Localization.HippotherapyLandingPageHippoventionCenterSection.GetByEntityId;

public class GetHippotherapyLandingPageHippoventionCenterSectionLocalizationByEntityIdHandler
    : IRequestHandler<GetHippotherapyLandingPageHippoventionCenterSectionLocalizationByEntityIdQuery, Result<List<HippotherapyLandingPageHippoventionCenterSectionLocalizationDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetHippotherapyLandingPageHippoventionCenterSectionLocalizationByEntityIdHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<List<HippotherapyLandingPageHippoventionCenterSectionLocalizationDto>>> Handle(
        GetHippotherapyLandingPageHippoventionCenterSectionLocalizationByEntityIdQuery request,
        CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<HippotherapyLandingPageHippoventionCenterSectionLocalization>
        {
            Filter = l => l.EntityId == request.Id,
            Include = l => l.Include(loc => loc.Language),
            AsNoTracking = true,
        };

        var entities = await _repositoryWrapper.HippotherapyLandingPageHippoventionCenterSectionLocalizationsRepository.GetAllAsync(queryOptions);
        var responseDto = _mapper.Map<List<HippotherapyLandingPageHippoventionCenterSectionLocalizationDto>>(entities);
        return Result.Ok(responseDto);
    }
}
