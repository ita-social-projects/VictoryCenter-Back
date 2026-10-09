using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAdvantagesSection;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Queries.Admin.Localization.HippotherapyLandingPageAdvantagesSection.GetByEntityId;

public class GetHippotherapyLandingPageAdvantagesSectionLocalizationByEntityIdHandler
    : IRequestHandler<GetHippotherapyLandingPageAdvantagesSectionLocalizationByEntityIdQuery, Result<List<HippotherapyLandingPageAdvantagesSectionLocalizationDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetHippotherapyLandingPageAdvantagesSectionLocalizationByEntityIdHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<List<HippotherapyLandingPageAdvantagesSectionLocalizationDto>>> Handle(
        GetHippotherapyLandingPageAdvantagesSectionLocalizationByEntityIdQuery request,
        CancellationToken cancellationToken)
    {
        var queryOptions = new QueryOptions<HippotherapyLandingPageAdvantagesSectionLocalization>
        {
            Filter = l => l.EntityId == request.Id,
            Include = l => l.Include(loc => loc.Language),
            AsNoTracking = true,
        };

        var entities = (await _repositoryWrapper.HippotherapyLandingPageAdvantagesSectionLocalizationsRepository.GetAllAsync(queryOptions)).ToList();
        var responseDto = _mapper.Map<List<HippotherapyLandingPageAdvantagesSectionLocalizationDto>>(entities);

        if (responseDto.Count == 0)
        {
            return Result.Ok(responseDto);
        }

        var cardIds = (await _repositoryWrapper.HippotherapyLandingPageAdvantageCardsRepository
            .GetAllAsync(new QueryOptions<HippotherapyLandingPageAdvantageCard>
            {
                Filter = c => c.AdvantagesSectionId == request.Id,
                OrderByASC = c => c.Priority,
                AsNoTracking = true,
            }))
            .Select(c => c.Id)
            .ToList();

        var cardLocalizations = cardIds.Count == 0
            ? []
            : (await _repositoryWrapper.HippotherapyLandingPageAdvantageCardLocalizationsRepository
                .GetAllAsync(new QueryOptions<HippotherapyLandingPageAdvantageCardLocalization>
                {
                    Filter = l => cardIds.Contains(l.EntityId),
                    AsNoTracking = true,
                }))
                .ToList();

        var cardLocalizationsByKey = cardLocalizations.ToDictionary(l => (l.EntityId, l.LanguageId));

        for (var i = 0; i < entities.Count; i++)
        {
            var languageId = entities[i].LanguageId;
            responseDto[i].Cards = cardIds
                .Select(cardId => cardLocalizationsByKey.TryGetValue((cardId, languageId), out var localization)
                    ? _mapper.Map<HippotherapyLandingPageAdvantageCardLocalizationItemDto>(localization)
                    : new HippotherapyLandingPageAdvantageCardLocalizationItemDto { CardId = cardId, Description = string.Empty })
                .ToList();
        }

        return Result.Ok(responseDto);
    }
}
