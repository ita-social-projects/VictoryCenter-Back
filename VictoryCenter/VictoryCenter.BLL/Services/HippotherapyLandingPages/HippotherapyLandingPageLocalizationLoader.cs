using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAdvantagesSection;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAnalysisSection;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAnotherQuoteSection;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageDescriptionSection;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageEthicsSection;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageHippoventionCenterSection;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageHippoventionSection;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageIntroSection;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageParticipantsSection;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageQuoteSection;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageScientificReferencesSection;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Services.HippotherapyLandingPages;

public class HippotherapyLandingPageLocalizationLoader : IHippotherapyLandingPageLocalizationLoader
{
    private readonly IRepositoryWrapper _repositories;
    private readonly IMapper _mapper;

    public HippotherapyLandingPageLocalizationLoader(IRepositoryWrapper repositories, IMapper mapper)
    {
        _repositories = repositories;
        _mapper = mapper;
    }

    public async Task<HippotherapyLandingPageLocalizationSource> LoadAsync(
        DAL.Entities.HippotherapyLandingPage page,
        CancellationToken cancellationToken)
    {
        var intro = await LoadSectionAsync<
            HippotherapyLandingPageIntroSectionLocalization,
            HippotherapyLandingPageIntroSectionLocalizationDto>(
            _repositories.HippotherapyLandingPageIntroSectionLocalizationsRepository,
            l => l.EntityId == page.IntroSection.Id,
            l => l.Include(x => x.Language));

        var description = await LoadSectionAsync<
            HippotherapyLandingPageDescriptionSectionLocalization,
            HippotherapyLandingPageDescriptionSectionLocalizationDto>(
            _repositories.HippotherapyLandingPageDescriptionSectionLocalizationsRepository,
            l => l.EntityId == page.DescriptionSection.Id,
            l => l.Include(x => x.Language));

        var quote = await LoadSectionAsync<
            HippotherapyLandingPageQuoteSectionLocalization,
            HippotherapyLandingPageQuoteSectionLocalizationDto>(
            _repositories.HippotherapyLandingPageQuoteSectionLocalizationsRepository,
            l => l.EntityId == page.QuoteSection.Id,
            l => l.Include(x => x.Language));

        var hippovention = await LoadSectionAsync<
            HippotherapyLandingPageHippoventionSectionLocalization,
            HippotherapyLandingPageHippoventionSectionLocalizationDto>(
            _repositories.HippotherapyLandingPageHippoventionSectionLocalizationsRepository,
            l => l.EntityId == page.HippoventionSection.Id,
            l => l.Include(x => x.Language));

        var center = await LoadSectionAsync<
            HippotherapyLandingPageHippoventionCenterSectionLocalization,
            HippotherapyLandingPageHippoventionCenterSectionLocalizationDto>(
            _repositories.HippotherapyLandingPageHippoventionCenterSectionLocalizationsRepository,
            l => l.EntityId == page.HippoventionCenterSection.Id,
            l => l.Include(x => x.Language));

        var analysis = await LoadSectionAsync<
            HippotherapyLandingPageAnalysisSectionLocalization,
            HippotherapyLandingPageAnalysisSectionLocalizationDto>(
            _repositories.HippotherapyLandingPageAnalysisSectionLocalizationsRepository,
            l => l.EntityId == page.AnalysisSection.Id,
            l => l.Include(x => x.Language));

        var another = await LoadSectionAsync<
            HippotherapyLandingPageAnotherQuoteSectionLocalization,
            HippotherapyLandingPageAnotherQuoteSectionLocalizationDto>(
            _repositories.HippotherapyLandingPageAnotherQuoteSectionLocalizationsRepository,
            l => l.EntityId == page.AnotherQuoteSection.Id,
            l => l.Include(x => x.Language));

        var advantages = await LoadAdvantagesAsync(page.AdvantagesSection.Id);
        var participants = await LoadParticipantsAsync(page.ParticipantsSection.Id);
        var references = await LoadReferencesAsync(page.ScientificReferencesSection.Id);
        var ethics = await LoadEthicsAsync(page.EthicsSection.Id);

        return new HippotherapyLandingPageLocalizationSource(
            intro,
            description,
            quote,
            hippovention,
            center,
            advantages,
            analysis,
            references,
            another,
            participants,
            ethics);
    }

    private async Task<List<HippotherapyLandingPageAdvantagesSectionLocalizationDto>> LoadAdvantagesAsync(
        long sectionId)
    {
        var dtos = await LoadSectionAsync<
            HippotherapyLandingPageAdvantagesSectionLocalization,
            HippotherapyLandingPageAdvantagesSectionLocalizationDto>(
            _repositories.HippotherapyLandingPageAdvantagesSectionLocalizationsRepository,
            l => l.EntityId == sectionId,
            l => l.Include(x => x.Language));

        if (dtos.Count == 0)
        {
            return dtos;
        }

        var cardIds = await GetOrderedIdsAsync(
            _repositories.HippotherapyLandingPageAdvantageCardsRepository,
            c => c.AdvantagesSectionId == sectionId,
            c => c.Priority,
            c => c.Id);

        var localizations = await GetItemLocalizationsAsync(
            _repositories.HippotherapyLandingPageAdvantageCardLocalizationsRepository,
            l => cardIds.Contains(l.EntityId),
            l => (l.EntityId, l.LanguageId));

        foreach (var dto in dtos)
        {
            dto.Cards = cardIds
                .Select(id => localizations.TryGetValue((id, dto.LocalizationInfoDto.Id), out var localization)
                    ? _mapper.Map<HippotherapyLandingPageAdvantageCardLocalizationItemDto>(localization)
                    : new HippotherapyLandingPageAdvantageCardLocalizationItemDto
                    {
                        CardId = id,
                        Description = string.Empty,
                    })
                .ToList();
        }

        return dtos;
    }

    private async Task<List<HippotherapyLandingPageParticipantsSectionLocalizationDto>> LoadParticipantsAsync(
        long sectionId)
    {
        var dtos = await LoadSectionAsync<
            HippotherapyLandingPageParticipantsSectionLocalization,
            HippotherapyLandingPageParticipantsSectionLocalizationDto>(
            _repositories.HippotherapyLandingPageParticipantsSectionLocalizationsRepository,
            l => l.EntityId == sectionId,
            l => l.Include(x => x.Language));

        if (dtos.Count == 0)
        {
            return dtos;
        }

        var cardIds = await GetOrderedIdsAsync(
            _repositories.HippotherapyLandingPageParticipantCardsRepository,
            c => c.ParticipantsSectionId == sectionId,
            c => c.Priority,
            c => c.Id);

        var localizations = await GetItemLocalizationsAsync(
            _repositories.HippotherapyLandingPageParticipantCardLocalizationsRepository,
            l => cardIds.Contains(l.EntityId),
            l => (l.EntityId, l.LanguageId));

        foreach (var dto in dtos)
        {
            dto.Cards = cardIds
                .Select(id => localizations.TryGetValue((id, dto.LocalizationInfoDto.Id), out var localization)
                    ? _mapper.Map<HippotherapyLandingPageParticipantCardLocalizationItemDto>(localization)
                    : new HippotherapyLandingPageParticipantCardLocalizationItemDto
                    {
                        CardId = id,
                        Description = string.Empty,
                    })
                .ToList();
        }

        return dtos;
    }

    private async Task<List<HippotherapyLandingPageScientificReferencesSectionLocalizationDto>> LoadReferencesAsync(
        long sectionId)
    {
        var dtos = await LoadSectionAsync<
            HippotherapyLandingPageScientificReferencesSectionLocalization,
            HippotherapyLandingPageScientificReferencesSectionLocalizationDto>(
            _repositories.HippotherapyLandingPageScientificReferencesSectionLocalizationsRepository,
            l => l.EntityId == sectionId,
            l => l.Include(x => x.Language));

        if (dtos.Count == 0)
        {
            return dtos;
        }

        var referenceIds = await GetOrderedIdsAsync(
            _repositories.HippotherapyLandingPageScientificReferencesRepository,
            r => r.ScientificReferencesSectionId == sectionId,
            r => r.Priority,
            r => r.Id);

        var localizations = await GetItemLocalizationsAsync(
            _repositories.HippotherapyLandingPageScientificReferenceLocalizationsRepository,
            l => referenceIds.Contains(l.EntityId),
            l => (l.EntityId, l.LanguageId));

        foreach (var dto in dtos)
        {
            dto.References = referenceIds
                .Select(id => localizations.TryGetValue((id, dto.LocalizationInfoDto.Id), out var localization)
                    ? _mapper.Map<HippotherapyLandingPageScientificReferenceLocalizationItemDto>(localization)
                    : new HippotherapyLandingPageScientificReferenceLocalizationItemDto
                    {
                        ReferenceId = id,
                        Name = string.Empty,
                    })
                .ToList();
        }

        return dtos;
    }

    private async Task<List<HippotherapyLandingPageEthicsSectionLocalizationDto>> LoadEthicsAsync(long sectionId)
    {
        var dtos = await LoadSectionAsync<
            HippotherapyLandingPageEthicsSectionLocalization,
            HippotherapyLandingPageEthicsSectionLocalizationDto>(
            _repositories.HippotherapyLandingPageEthicsSectionLocalizationsRepository,
            l => l.EntityId == sectionId,
            l => l.Include(x => x.Language));

        if (dtos.Count == 0)
        {
            return dtos;
        }

        var principleIds = await GetOrderedIdsAsync(
            _repositories.HippotherapyLandingPageEthicsPrinciplesRepository,
            p => p.EthicsSectionId == sectionId,
            p => p.Priority,
            p => p.Id);

        var localizations = await GetItemLocalizationsAsync(
            _repositories.HippotherapyLandingPageEthicsPrincipleLocalizationsRepository,
            l => principleIds.Contains(l.EntityId),
            l => (l.EntityId, l.LanguageId));

        foreach (var dto in dtos)
        {
            dto.Principles = principleIds
                .Select(id => localizations.TryGetValue((id, dto.LocalizationInfoDto.Id), out var localization)
                    ? _mapper.Map<HippotherapyLandingPageEthicsPrincipleLocalizationItemDto>(localization)
                    : new HippotherapyLandingPageEthicsPrincipleLocalizationItemDto
                    {
                        PrincipleId = id,
                        Text = string.Empty,
                    })
                .ToList();
        }

        return dtos;
    }

    private async Task<List<TDto>> LoadSectionAsync<TEntity, TDto>(
        IRepositoryBase<TEntity> repository,
        Expression<Func<TEntity, bool>> filter,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include)
        where TEntity : class
    {
        var entities = (await repository.GetAllAsync(new QueryOptions<TEntity>
        {
            Filter = filter,
            Include = include,
            AsNoTracking = true,
        })).ToList();

        return _mapper.Map<List<TDto>>(entities);
    }

    private static async Task<List<long>> GetOrderedIdsAsync<TItem>(
        IRepositoryBase<TItem> repository,
        Expression<Func<TItem, bool>> filter,
        Expression<Func<TItem, object>> orderBy,
        Func<TItem, long> idSelector)
        where TItem : class
    {
        var items = await repository.GetAllAsync(new QueryOptions<TItem>
        {
            Filter = filter,
            OrderByASC = orderBy,
            AsNoTracking = true,
        });

        return items.Select(idSelector).ToList();
    }

    private static async Task<Dictionary<(long EntityId, long LanguageId), TLocalization>> GetItemLocalizationsAsync<
        TLocalization>(
        IRepositoryBase<TLocalization> repository,
        Expression<Func<TLocalization, bool>> filter,
        Func<TLocalization, (long EntityId, long LanguageId)> keySelector)
        where TLocalization : class
    {
        var items = await repository.GetAllAsync(new QueryOptions<TLocalization>
        {
            Filter = filter,
            AsNoTracking = true,
        });

        return items.ToDictionary(keySelector);
    }
}
