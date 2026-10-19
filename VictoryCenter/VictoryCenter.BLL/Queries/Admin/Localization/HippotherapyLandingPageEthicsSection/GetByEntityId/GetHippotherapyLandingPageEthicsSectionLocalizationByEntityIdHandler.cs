using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageEthicsSection;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.Localization.HippotherapyLandingPageEthicsSection.GetByEntityId;

public class GetHippotherapyLandingPageEthicsSectionLocalizationByEntityIdHandler
    : IRequestHandler<GetHippotherapyLandingPageEthicsSectionLocalizationByEntityIdQuery, Result<List<HippotherapyLandingPageEthicsSectionLocalizationDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetHippotherapyLandingPageEthicsSectionLocalizationByEntityIdHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<List<HippotherapyLandingPageEthicsSectionLocalizationDto>>> Handle(
        GetHippotherapyLandingPageEthicsSectionLocalizationByEntityIdQuery request,
        CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<HippotherapyLandingPageEthicsSectionLocalization>
        {
            Filter = l => l.EntityId == request.Id,
            Include = l => l.Include(loc => loc.Language),
            AsNoTracking = true,
        };

        var entities = (await _repositoryWrapper.HippotherapyLandingPageEthicsSectionLocalizationsRepository.GetAllAsync(queryOptions)).ToList();
        var responseDto = _mapper.Map<List<HippotherapyLandingPageEthicsSectionLocalizationDto>>(entities);

        if (responseDto.Count == 0)
        {
            return Result.Ok(responseDto);
        }

        var principleIds = (await _repositoryWrapper.HippotherapyLandingPageEthicsPrinciplesRepository
            .GetAllAsync(new QueryOptions<HippotherapyLandingPageEthicsPrinciple>
            {
                Filter = r => r.EthicsSectionId == request.Id,
                OrderByASC = r => r.Priority,
                AsNoTracking = true,
            }))
            .Select(r => r.Id)
            .ToList();

        var principleLocalizations = principleIds.Count == 0
            ? []
            : (await _repositoryWrapper.HippotherapyLandingPageEthicsPrincipleLocalizationsRepository
                .GetAllAsync(new QueryOptions<HippotherapyLandingPageEthicsPrincipleLocalization>
                {
                    Filter = l => principleIds.Contains(l.EntityId),
                    AsNoTracking = true,
                }))
                .ToList();

        var principleLocalizationsByKey = principleLocalizations.ToDictionary(l => (l.EntityId, l.LanguageId));

        for (var i = 0; i < entities.Count; i++)
        {
            var languageId = entities[i].LanguageId;
            responseDto[i].Principles = principleIds
                .Select(principleId => principleLocalizationsByKey.TryGetValue((principleId, languageId), out var localization)
                    ? _mapper.Map<HippotherapyLandingPageEthicsPrincipleLocalizationItemDto>(localization)
                    : new HippotherapyLandingPageEthicsPrincipleLocalizationItemDto { PrincipleId = principleId, Text = string.Empty })
                .ToList();
        }

        return Result.Ok(responseDto);
    }
}
