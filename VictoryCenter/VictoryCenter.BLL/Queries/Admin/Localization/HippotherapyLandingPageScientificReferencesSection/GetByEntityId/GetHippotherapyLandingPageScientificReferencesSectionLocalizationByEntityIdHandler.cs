using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageScientificReferencesSection;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.Localization.HippotherapyLandingPageScientificReferencesSection.GetByEntityId;

public class GetHippotherapyLandingPageScientificReferencesSectionLocalizationByEntityIdHandler
    : IRequestHandler<GetHippotherapyLandingPageScientificReferencesSectionLocalizationByEntityIdQuery, Result<List<HippotherapyLandingPageScientificReferencesSectionLocalizationDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetHippotherapyLandingPageScientificReferencesSectionLocalizationByEntityIdHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<List<HippotherapyLandingPageScientificReferencesSectionLocalizationDto>>> Handle(
        GetHippotherapyLandingPageScientificReferencesSectionLocalizationByEntityIdQuery request,
        CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<HippotherapyLandingPageScientificReferencesSectionLocalization>
        {
            Filter = l => l.EntityId == request.Id,
            Include = l => l.Include(loc => loc.Language),
            AsNoTracking = true,
        };

        var entities = (await _repositoryWrapper.HippotherapyLandingPageScientificReferencesSectionLocalizationsRepository.GetAllAsync(queryOptions)).ToList();
        var responseDto = _mapper.Map<List<HippotherapyLandingPageScientificReferencesSectionLocalizationDto>>(entities);

        if (responseDto.Count == 0)
        {
            return Result.Ok(responseDto);
        }

        var referenceIds = (await _repositoryWrapper.HippotherapyLandingPageScientificReferencesRepository
            .GetAllAsync(new QueryOptions<HippotherapyLandingPageScientificReference>
            {
                Filter = r => r.ScientificReferencesSectionId == request.Id,
                OrderByASC = r => r.Priority,
                AsNoTracking = true,
            }))
            .Select(r => r.Id)
            .ToList();

        var referenceLocalizations = referenceIds.Count == 0
            ? []
            : (await _repositoryWrapper.HippotherapyLandingPageScientificReferenceLocalizationsRepository
                .GetAllAsync(new QueryOptions<HippotherapyLandingPageScientificReferenceLocalization>
                {
                    Filter = l => referenceIds.Contains(l.EntityId),
                    AsNoTracking = true,
                }))
                .ToList();

        var referenceLocalizationsByKey = referenceLocalizations.ToDictionary(l => (l.EntityId, l.LanguageId));

        for (var i = 0; i < entities.Count; i++)
        {
            var languageId = entities[i].LanguageId;
            responseDto[i].References = referenceIds
                .Select(referenceId => referenceLocalizationsByKey.TryGetValue((referenceId, languageId), out var localization)
                    ? _mapper.Map<HippotherapyLandingPageScientificReferenceLocalizationItemDto>(localization)
                    : new HippotherapyLandingPageScientificReferenceLocalizationItemDto { ReferenceId = referenceId, Name = string.Empty })
                .ToList();
        }

        return Result.Ok(responseDto);
    }
}
