using AutoMapper;
using FluentResults;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAdvantagesSection;
using VictoryCenter.BLL.Interfaces.HippotherapyLandingPages;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Services.HippotherapyLandingPages;

public class HippotherapyLandingPageAdvantageCardLocalizationUpdater : IHippotherapyLandingPageAdvantageCardLocalizationUpdater
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILocalizationService<HippotherapyLandingPageAdvantageCard, HippotherapyLandingPageAdvantageCardLocalization> _cardLocalizationService;

    public HippotherapyLandingPageAdvantageCardLocalizationUpdater(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        ILocalizationService<HippotherapyLandingPageAdvantageCard, HippotherapyLandingPageAdvantageCardLocalization> cardLocalizationService)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _cardLocalizationService = cardLocalizationService;
    }

    public async Task<Result<List<HippotherapyLandingPageAdvantageCardLocalizationItemDto>>> UpsertCardsAsync(
        HippotherapyLandingPageAdvantagesSection section,
        List<UpdateHippotherapyLandingPageAdvantageCardLocalizationItemDto> cards,
        long languageId)
    {
        var sectionCardIds = section.AdvantageCards.Select(c => c.Id).ToHashSet();
        var invalidCardIds = cards
            .Select(c => c.CardId)
            .Where(id => !sectionCardIds.Contains(id))
            .ToList();

        if (invalidCardIds.Count > 0)
        {
            return Result.Fail(ErrorMessagesConstants.NotFound(invalidCardIds, typeof(HippotherapyLandingPageAdvantageCard)));
        }

        if (cards.Count == 0)
        {
            return Result.Ok(new List<HippotherapyLandingPageAdvantageCardLocalizationItemDto>());
        }

        var cardIds = cards.Select(c => c.CardId).ToList();
        var existingByCardId = (await _repositoryWrapper.HippotherapyLandingPageAdvantageCardLocalizationsRepository
            .GetAllAsync(new QueryOptions<HippotherapyLandingPageAdvantageCardLocalization>
            {
                Filter = l => l.LanguageId == languageId && cardIds.Contains(l.EntityId),
                AsNoTracking = true
            }))
            .ToDictionary(l => l.EntityId);

        var entities = cards.Select(dto =>
        {
            var entity = _mapper.Map<HippotherapyLandingPageAdvantageCardLocalization>(dto);
            entity.LanguageId = languageId;

            if (existingByCardId.TryGetValue(entity.EntityId, out var existing))
            {
                entity.TranslationStatus = TranslationStatus.Relevant;
                entity.CreatedAt = existing.CreatedAt;
            }
            else
            {
                entity.CreatedAt = DateTimeOffset.UtcNow;
            }

            return entity;
        }).ToList();

        var toUpdate = entities.Where(e => existingByCardId.ContainsKey(e.EntityId)).ToList();
        var toCreate = entities.Where(e => !existingByCardId.ContainsKey(e.EntityId)).ToList();

        if (toUpdate.Count > 0)
        {
            await _cardLocalizationService.TrackEntityLocalizationAsync(toUpdate, isUpdate: true);
        }

        if (toCreate.Count > 0)
        {
            await _cardLocalizationService.TrackEntityLocalizationAsync(toCreate, isUpdate: false);
        }

        if (await _repositoryWrapper.SaveChangesAsync() <= 0)
        {
            throw new InvalidOperationException();
        }

        var results = entities.Select(e => _mapper.Map<HippotherapyLandingPageAdvantageCardLocalizationItemDto>(e)).ToList();
        return Result.Ok(results);
    }
}
