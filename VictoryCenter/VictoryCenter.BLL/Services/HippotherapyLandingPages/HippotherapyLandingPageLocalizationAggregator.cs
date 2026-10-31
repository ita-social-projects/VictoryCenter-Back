using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPage;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAdvantagesSection;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageEthicsSection;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageParticipantsSection;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageScientificReferencesSection;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Enums;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Services.HippotherapyLandingPages;

public static class HippotherapyLandingPageLocalizationAggregator
{
    public static List<HippotherapyLandingPageAggregatedLocalizationDto> BuildLocalizations(
        HippotherapyLandingPageLocalizationSource s)
    {
        var intro = ByLanguage(s.Intro, x => x.LocalizationInfoDto.Id);
        var description = ByLanguage(s.Description, x => x.LocalizationInfoDto.Id);
        var quote = ByLanguage(s.Quote, x => x.LocalizationInfoDto.Id);
        var hippovention = ByLanguage(s.Hippovention, x => x.LocalizationInfoDto.Id);
        var center = ByLanguage(s.HippoventionCenter, x => x.LocalizationInfoDto.Id);
        var advantages = ByLanguage(s.Advantages, x => x.LocalizationInfoDto.Id);
        var analysis = ByLanguage(s.Analysis, x => x.LocalizationInfoDto.Id);
        var references = ByLanguage(s.ScientificReferences, x => x.LocalizationInfoDto.Id);
        var another = ByLanguage(s.AnotherQuote, x => x.LocalizationInfoDto.Id);
        var participants = ByLanguage(s.Participants, x => x.LocalizationInfoDto.Id);
        var ethics = ByLanguage(s.Ethics, x => x.LocalizationInfoDto.Id);

        var languages = new IEnumerable<LocalizationInfoDto>[]
            {
                s.Intro.Select(x => x.LocalizationInfoDto),
                s.Description.Select(x => x.LocalizationInfoDto),
                s.Quote.Select(x => x.LocalizationInfoDto),
                s.Hippovention.Select(x => x.LocalizationInfoDto),
                s.HippoventionCenter.Select(x => x.LocalizationInfoDto),
                s.Advantages.Select(x => x.LocalizationInfoDto),
                s.Analysis.Select(x => x.LocalizationInfoDto),
                s.ScientificReferences.Select(x => x.LocalizationInfoDto),
                s.AnotherQuote.Select(x => x.LocalizationInfoDto),
                s.Participants.Select(x => x.LocalizationInfoDto),
                s.Ethics.Select(x => x.LocalizationInfoDto),
            }
            .SelectMany(x => x)
            .GroupBy(x => x.Id)
            .Select(g => g.First())
            .OrderBy(x => x.Id);

        return languages.Select(lang =>
        {
            var id = lang.Id;
            var i = intro.GetValueOrDefault(id);
            var d = description.GetValueOrDefault(id);
            var q = quote.GetValueOrDefault(id);
            var h = hippovention.GetValueOrDefault(id);
            var c = center.GetValueOrDefault(id);
            var a = advantages.GetValueOrDefault(id);
            var an = analysis.GetValueOrDefault(id);
            var r = references.GetValueOrDefault(id);
            var aq = another.GetValueOrDefault(id);
            var p = participants.GetValueOrDefault(id);
            var e = ethics.GetValueOrDefault(id);

            var statuses = new TranslationStatus?[]
            {
                i?.TranslationStatus, d?.TranslationStatus, q?.TranslationStatus, h?.TranslationStatus,
                c?.TranslationStatus, a is null ? null : BlockStatus(a), an?.TranslationStatus,
                r is null ? null : BlockStatus(r), aq?.TranslationStatus,
                p is null ? null : BlockStatus(p), e is null ? null : BlockStatus(e),
            };

            return new HippotherapyLandingPageAggregatedLocalizationDto
            {
                LocalizationInfoDto = lang,
                TranslationStatus = statuses.Contains(TranslationStatus.Outdated)
                    ? TranslationStatus.Outdated
                    : TranslationStatus.Relevant,
                IntroSection = i is null ? null : new TextLocalizationDto(i.Title, i.Description),
                DescriptionSection = d is null ? null : new TextLocalizationDto(d.Title, d.Description),
                QuoteSection = q is null ? null : new QuoteLocalizationDto(q.QuoteText, q.AuthorName),
                HippoventionSection = h is null ? null : new TextLocalizationDto(h.Title, h.Description),
                HippoventionCenterSection = c is null
                    ? null
                    : new HippoventionCenterLocalizationDto(c.Title, c.Description, c.Pros),
                AdvantagesSection = a is null
                    ? null
                    : ToGallery(a.Title, a.Cards, x => x.CardId, x => x.Description),
                AnalysisSection = an is null ? null : new TextLocalizationDto(an.Title, an.Description),
                ScientificReferencesSection = r is null
                    ? null
                    : new ScientificReferencesLocalizationDto(
                        r.Title,
                        r.Description,
                        r.References.Select(x => new ReferenceLocalizationDto(x.ReferenceId, x.Name)).ToList()),
                AnotherQuoteSection = aq is null ? null : new QuoteLocalizationDto(aq.QuoteText, aq.AuthorName),
                ParticipantsSection = p is null
                    ? null
                    : ToGallery(p.Title, p.Cards, x => x.CardId, x => x.Description),
                EthicsSection = e is null
                    ? null
                    : new EthicsLocalizationDto(
                        e.Title,
                        e.Description,
                        e.Principles.Select(x => new PrincipleLocalizationDto(x.PrincipleId, x.Text)).ToList()),
            };
        }).ToList();
    }

    public static List<HippotherapyLandingPageTranslationStatusDto> BuildStatuses(
        HippotherapyLandingPageLocalizationSource s)
    {
        var result = new List<HippotherapyLandingPageTranslationStatusDto>();

        void Add<T>(
            HippotherapyLandingPageBlock block,
            IEnumerable<T> items,
            Func<T, long> languageId,
            Func<T, TranslationStatus?> status)
            => result.AddRange(items.Select(x =>
                new HippotherapyLandingPageTranslationStatusDto(block, languageId(x), status(x))));

        Add(HippotherapyLandingPageBlock.Intro, s.Intro, x => x.LocalizationInfoDto.Id, x => x.TranslationStatus);
        Add(HippotherapyLandingPageBlock.Description, s.Description, x => x.LocalizationInfoDto.Id, x => x.TranslationStatus);
        Add(HippotherapyLandingPageBlock.Quote, s.Quote, x => x.LocalizationInfoDto.Id, x => x.TranslationStatus);
        Add(HippotherapyLandingPageBlock.Hippovention, s.Hippovention, x => x.LocalizationInfoDto.Id, x => x.TranslationStatus);
        Add(HippotherapyLandingPageBlock.HippoventionCenter, s.HippoventionCenter, x => x.LocalizationInfoDto.Id, x => x.TranslationStatus);
        Add(HippotherapyLandingPageBlock.Advantages, s.Advantages, x => x.LocalizationInfoDto.Id, x => BlockStatus(x));
        Add(HippotherapyLandingPageBlock.Analysis, s.Analysis, x => x.LocalizationInfoDto.Id, x => x.TranslationStatus);
        Add(HippotherapyLandingPageBlock.ScientificReferences, s.ScientificReferences, x => x.LocalizationInfoDto.Id, x => BlockStatus(x));
        Add(HippotherapyLandingPageBlock.AnotherQuote, s.AnotherQuote, x => x.LocalizationInfoDto.Id, x => x.TranslationStatus);
        Add(HippotherapyLandingPageBlock.Participants, s.Participants, x => x.LocalizationInfoDto.Id, x => BlockStatus(x));
        Add(HippotherapyLandingPageBlock.Ethics, s.Ethics, x => x.LocalizationInfoDto.Id, x => BlockStatus(x));

        return result.OrderBy(x => x.Block).ThenBy(x => x.LanguageId).ToList();
    }

    private static Dictionary<long, T> ByLanguage<T>(IEnumerable<T> items, Func<T, long> languageId)
        => items.GroupBy(languageId).ToDictionary(g => g.Key, g => g.First());

    private static GalleryLocalizationDto ToGallery<TCard>(
        string title, IEnumerable<TCard> cards, Func<TCard, long> cardId, Func<TCard, string> description)
        => new(title, cards.Select(c => new CardLocalizationDto(cardId(c), description(c))).ToList());

    private static TranslationStatus BlockStatus(HippotherapyLandingPageAdvantagesSectionLocalizationDto x)
        => AnyOutdated(x.TranslationStatus, x.Cards.Select(c => c.TranslationStatus));

    private static TranslationStatus BlockStatus(HippotherapyLandingPageParticipantsSectionLocalizationDto x)
        => AnyOutdated(x.TranslationStatus, x.Cards.Select(c => c.TranslationStatus));

    private static TranslationStatus BlockStatus(HippotherapyLandingPageScientificReferencesSectionLocalizationDto x)
        => AnyOutdated(x.TranslationStatus, x.References.Select(r => r.TranslationStatus));

    private static TranslationStatus BlockStatus(HippotherapyLandingPageEthicsSectionLocalizationDto x)
        => AnyOutdated(x.TranslationStatus, x.Principles.Select(p => p.TranslationStatus));

    private static TranslationStatus AnyOutdated(TranslationStatus own, IEnumerable<TranslationStatus?> nested)
        => own == TranslationStatus.Outdated || nested.Any(s => s == TranslationStatus.Outdated)
            ? TranslationStatus.Outdated
            : TranslationStatus.Relevant;
}
