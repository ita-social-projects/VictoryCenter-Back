using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAnotherQuoteSection;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.Localization.HippotherapyLandingPageAnotherQuoteSection.GetByEntityId;

public class GetHippotherapyLandingPageAnotherQuoteSectionLocalizationByEntityIdHandler
    : IRequestHandler<GetHippotherapyLandingPageAnotherQuoteSectionLocalizationByEntityIdQuery, Result<List<HippotherapyLandingPageAnotherQuoteSectionLocalizationDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetHippotherapyLandingPageAnotherQuoteSectionLocalizationByEntityIdHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<List<HippotherapyLandingPageAnotherQuoteSectionLocalizationDto>>> Handle(
        GetHippotherapyLandingPageAnotherQuoteSectionLocalizationByEntityIdQuery request,
        CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<HippotherapyLandingPageAnotherQuoteSectionLocalization>
        {
            Filter = l => l.EntityId == request.Id,
            Include = l => l.Include(loc => loc.Language),
            AsNoTracking = true,
        };

        var entities = await _repositoryWrapper.HippotherapyLandingPageAnotherQuoteSectionLocalizationsRepository.GetAllAsync(queryOptions);
        var responseDto = _mapper.Map<List<HippotherapyLandingPageAnotherQuoteSectionLocalizationDto>>(entities);
        return Result.Ok(responseDto);
    }
}
