using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAnalysisSection;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.Localization.HippotherapyLandingPageAnalysisSection.GetByEntityId;

public class GetHippotherapyLandingPageAnalysisSectionLocalizationByEntityIdHandler
    : IRequestHandler<GetHippotherapyLandingPageAnalysisSectionLocalizationByEntityIdQuery, Result<List<HippotherapyLandingPageAnalysisSectionLocalizationDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetHippotherapyLandingPageAnalysisSectionLocalizationByEntityIdHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<List<HippotherapyLandingPageAnalysisSectionLocalizationDto>>> Handle(
        GetHippotherapyLandingPageAnalysisSectionLocalizationByEntityIdQuery request,
        CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<HippotherapyLandingPageAnalysisSectionLocalization>
        {
            Filter = l => l.EntityId == request.Id,
            Include = l => l.Include(loc => loc.Language),
            AsNoTracking = true,
        };

        var entities = await _repositoryWrapper.HippotherapyLandingPageAnalysisSectionLocalizationsRepository.GetAllAsync(queryOptions);
        var responseDto = _mapper.Map<List<HippotherapyLandingPageAnalysisSectionLocalizationDto>>(entities);
        return Result.Ok(responseDto);
    }
}
