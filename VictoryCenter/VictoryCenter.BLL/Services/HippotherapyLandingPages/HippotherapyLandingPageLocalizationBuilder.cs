using AutoMapper;
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
using VictoryCenter.BLL.Helpers;

namespace VictoryCenter.BLL.Services.HippotherapyLandingPages;

public class HippotherapyLandingPageLocalizationBuilder : IHippotherapyLandingPageLocalizationBuilder
{
    private readonly IMapper _mapper;

    public HippotherapyLandingPageLocalizationBuilder(IMapper mapper)
    {
        _mapper = mapper;
    }

    public HippotherapyLandingPageLocalizationSource Build(DAL.Entities.HippotherapyLandingPage page)
    {
        ArgumentNullException.ThrowIfNull(page);

        if (!page.HasAllSections())
        {
            throw new ArgumentException(
                "Hippotherapy landing page must contain all sections (use IncludeFullGraph).",
                nameof(page));
        }

        return new HippotherapyLandingPageLocalizationSource(
            Map<HippotherapyLandingPageIntroSectionLocalizationDto>(page.IntroSection!.Localizations),
            Map<HippotherapyLandingPageDescriptionSectionLocalizationDto>(page.DescriptionSection!.Localizations),
            Map<HippotherapyLandingPageQuoteSectionLocalizationDto>(page.QuoteSection!.Localizations),
            Map<HippotherapyLandingPageHippoventionSectionLocalizationDto>(page.HippoventionSection!.Localizations),
            Map<HippotherapyLandingPageHippoventionCenterSectionLocalizationDto>(
                page.HippoventionCenterSection!.Localizations),
            BuildAdvantages(page),
            Map<HippotherapyLandingPageAnalysisSectionLocalizationDto>(page.AnalysisSection!.Localizations),
            BuildReferences(page),
            Map<HippotherapyLandingPageAnotherQuoteSectionLocalizationDto>(page.AnotherQuoteSection!.Localizations),
            BuildParticipants(page),
            BuildEthics(page));
    }

    private List<HippotherapyLandingPageAdvantagesSectionLocalizationDto> BuildAdvantages(
        DAL.Entities.HippotherapyLandingPage page)
    {
        var section = page.AdvantagesSection!;
        var dtos = Map<HippotherapyLandingPageAdvantagesSectionLocalizationDto>(section.Localizations);

        FillItems(
            dtos,
            section.AdvantageCards.OrderBy(c => c.Priority).Select(c => c.Id).ToList(),
            section.AdvantageCards.SelectMany(c => c.Localizations),
            dto => dto.LocalizationInfoDto.Id,
            (dto, items) => dto.Cards = items,
            id => new HippotherapyLandingPageAdvantageCardLocalizationItemDto
            {
                CardId = id,
                Description = string.Empty,
            });

        return dtos;
    }

    private List<HippotherapyLandingPageParticipantsSectionLocalizationDto> BuildParticipants(
        DAL.Entities.HippotherapyLandingPage page)
    {
        var section = page.ParticipantsSection!;
        var dtos = Map<HippotherapyLandingPageParticipantsSectionLocalizationDto>(section.Localizations);

        FillItems(
            dtos,
            section.ParticipantCards.OrderBy(c => c.Priority).Select(c => c.Id).ToList(),
            section.ParticipantCards.SelectMany(c => c.Localizations),
            dto => dto.LocalizationInfoDto.Id,
            (dto, items) => dto.Cards = items,
            id => new HippotherapyLandingPageParticipantCardLocalizationItemDto
            {
                CardId = id,
                Description = string.Empty,
            });

        return dtos;
    }

    private List<HippotherapyLandingPageScientificReferencesSectionLocalizationDto> BuildReferences(
        DAL.Entities.HippotherapyLandingPage page)
    {
        var section = page.ScientificReferencesSection!;
        var dtos = Map<HippotherapyLandingPageScientificReferencesSectionLocalizationDto>(section.Localizations);

        FillItems(
            dtos,
            section.ScientificReferences.OrderBy(r => r.Priority).Select(r => r.Id).ToList(),
            section.ScientificReferences.SelectMany(r => r.Localizations),
            dto => dto.LocalizationInfoDto.Id,
            (dto, items) => dto.References = items,
            id => new HippotherapyLandingPageScientificReferenceLocalizationItemDto
            {
                ReferenceId = id,
                Name = string.Empty,
            });

        return dtos;
    }

    private List<HippotherapyLandingPageEthicsSectionLocalizationDto> BuildEthics(
        DAL.Entities.HippotherapyLandingPage page)
    {
        var section = page.EthicsSection!;
        var dtos = Map<HippotherapyLandingPageEthicsSectionLocalizationDto>(section.Localizations);

        FillItems(
            dtos,
            section.EthicsPrinciples.OrderBy(p => p.Priority).Select(p => p.Id).ToList(),
            section.EthicsPrinciples.SelectMany(p => p.Localizations),
            dto => dto.LocalizationInfoDto.Id,
            (dto, items) => dto.Principles = items,
            id => new HippotherapyLandingPageEthicsPrincipleLocalizationItemDto
            {
                PrincipleId = id,
                Text = string.Empty,
            });

        return dtos;
    }

    private void FillItems<TSectionDto, TItemEntity, TItemDto>(
        List<TSectionDto> sectionDtos,
        IReadOnlyList<long> orderedItemIds,
        IEnumerable<TItemEntity> itemLocalizations,
        Func<TSectionDto, long> languageIdSelector,
        Action<TSectionDto, List<TItemDto>> assign,
        Func<long, TItemDto> createEmpty)
        where TItemEntity : class
    {
        if (sectionDtos.Count == 0 || orderedItemIds.Count == 0)
        {
            return;
        }

        var byKey = itemLocalizations
            .GroupBy(GetKey)
            .ToDictionary(g => g.Key, g => g.First());

        foreach (var dto in sectionDtos)
        {
            var languageId = languageIdSelector(dto);
            assign(
                dto,
                orderedItemIds
                    .Select(id => byKey.TryGetValue((id, languageId), out var localization)
                        ? _mapper.Map<TItemDto>(localization)
                        : createEmpty(id))
                    .ToList());
        }
    }

    private static (long EntityId, long LanguageId) GetKey<TItemEntity>(TItemEntity localization)
        where TItemEntity : class
    {
        dynamic item = localization;
        return ((long)item.EntityId, (long)item.LanguageId);
    }

    private List<TDto> Map<TDto>(object source) => _mapper.Map<List<TDto>>(source);
}
