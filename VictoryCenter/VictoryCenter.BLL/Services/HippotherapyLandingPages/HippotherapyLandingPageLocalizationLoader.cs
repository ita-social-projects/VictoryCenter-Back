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
        ArgumentNullException.ThrowIfNull(page);

        if (!HasAllSections(page))
        {
            throw new ArgumentException(
                "Hippotherapy landing page must contain all sections (use IncludeFullGraph).",
                nameof(page));
        }

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

        var advantages = await LoadAdvantagesAsync(
            page.AdvantagesSection.Id,
            page.AdvantagesSection.AdvantageCards.OrderBy(c => c.Priority).Select(c => c.Id).ToList());

        var participants = await LoadParticipantsAsync(
            page.ParticipantsSection.Id,
            page.ParticipantsSection.ParticipantCards.OrderBy(c => c.Priority).Select(c => c.Id).ToList());

        var references = await LoadReferencesAsync(
            page.ScientificReferencesSection.Id,
            page.ScientificReferencesSection.ScientificReferences
                .OrderBy(r => r.Priority)
                .Select(r => r.Id)
                .ToList());

        var ethics = await LoadEthicsAsync(
            page.EthicsSection.Id,
            page.EthicsSection.EthicsPrinciples.OrderBy(p => p.Priority).Select(p => p.Id).ToList());

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

    private static bool HasAllSections(DAL.Entities.HippotherapyLandingPage page)
        => page.IntroSection is not null
            && page.DescriptionSection is not null
            && page.QuoteSection is not null
            && page.HippoventionSection is not null
            && page.HippoventionCenterSection is not null
            && page.AdvantagesSection is not null
            && page.AnalysisSection is not null
            && page.ScientificReferencesSection is not null
            && page.AnotherQuoteSection is not null
            && page.ParticipantsSection is not null
            && page.EthicsSection is not null;

    private async Task<List<HippotherapyLandingPageAdvantagesSectionLocalizationDto>> LoadAdvantagesAsync(
        long sectionId,
        IReadOnlyList<long> cardIds)
    {
        var dtos = await LoadSectionAsync<
            HippotherapyLandingPageAdvantagesSectionLocalization,
            HippotherapyLandingPageAdvantagesSectionLocalizationDto>(
            _repositories.HippotherapyLandingPageAdvantagesSectionLocalizationsRepository,
            l => l.EntityId == sectionId,
            l => l.Include(x => x.Language));

        if (dtos.Count == 0 || cardIds.Count == 0)
        {
            return dtos;
        }

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
        long sectionId,
        IReadOnlyList<long> cardIds)
    {
        var dtos = await LoadSectionAsync<
            HippotherapyLandingPageParticipantsSectionLocalization,
            HippotherapyLandingPageParticipantsSectionLocalizationDto>(
            _repositories.HippotherapyLandingPageParticipantsSectionLocalizationsRepository,
            l => l.EntityId == sectionId,
            l => l.Include(x => x.Language));

        if (dtos.Count == 0 || cardIds.Count == 0)
        {
            return dtos;
        }

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
        long sectionId,
        IReadOnlyList<long> referenceIds)
    {
        var dtos = await LoadSectionAsync<
            HippotherapyLandingPageScientificReferencesSectionLocalization,
            HippotherapyLandingPageScientificReferencesSectionLocalizationDto>(
            _repositories.HippotherapyLandingPageScientificReferencesSectionLocalizationsRepository,
            l => l.EntityId == sectionId,
            l => l.Include(x => x.Language));

        if (dtos.Count == 0 || referenceIds.Count == 0)
        {
            return dtos;
        }

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

    private async Task<List<HippotherapyLandingPageEthicsSectionLocalizationDto>> LoadEthicsAsync(
        long sectionId,
        IReadOnlyList<long> principleIds)
    {
        var dtos = await LoadSectionAsync<
            HippotherapyLandingPageEthicsSectionLocalization,
            HippotherapyLandingPageEthicsSectionLocalizationDto>(
            _repositories.HippotherapyLandingPageEthicsSectionLocalizationsRepository,
            l => l.EntityId == sectionId,
            l => l.Include(x => x.Language));

        if (dtos.Count == 0 || principleIds.Count == 0)
        {
            return dtos;
        }

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

        return items
            .GroupBy(keySelector)
            .ToDictionary(g => g.Key, g => g.First());
    }
}
