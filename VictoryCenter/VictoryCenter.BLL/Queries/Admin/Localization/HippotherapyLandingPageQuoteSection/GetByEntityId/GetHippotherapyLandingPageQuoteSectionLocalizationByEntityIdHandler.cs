using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageQuoteSection;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.Localization.HippotherapyLandingPageQuoteSection.GetByEntityId;

public class GetHippotherapyLandingPageQuoteSectionLocalizationByEntityIdHandler
    : IRequestHandler<GetHippotherapyLandingPageQuoteSectionLocalizationByEntityIdQuery, Result<List<HippotherapyLandingPageQuoteSectionLocalizationDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetHippotherapyLandingPageQuoteSectionLocalizationByEntityIdHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<List<HippotherapyLandingPageQuoteSectionLocalizationDto>>> Handle(
        GetHippotherapyLandingPageQuoteSectionLocalizationByEntityIdQuery request,
        CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<HippotherapyLandingPageQuoteSectionLocalization>
        {
            Filter = l => l.EntityId == request.Id,
            Include = l => l.Include(loc => loc.Language),
            AsNoTracking = true,
        };

        var entities = await _repositoryWrapper.HippotherapyLandingPageQuoteSectionLocalizationsRepository.GetAllAsync(queryOptions);
        var responseDto = _mapper.Map<List<HippotherapyLandingPageQuoteSectionLocalizationDto>>(entities);
        return Result.Ok(responseDto);
    }
}
