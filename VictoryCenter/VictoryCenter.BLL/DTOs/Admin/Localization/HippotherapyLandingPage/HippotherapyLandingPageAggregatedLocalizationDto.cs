using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Enums;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPage;

public record HippotherapyLandingPageAggregatedLocalizationDto
{
    public LocalizationInfoDto LocalizationInfoDto { get; init; } = null!;

    public TranslationStatus TranslationStatus { get; init; }

    public TextLocalizationDto? IntroSection { get; init; }
    public TextLocalizationDto? DescriptionSection { get; init; }
    public QuoteLocalizationDto? QuoteSection { get; init; }
    public TextLocalizationDto? HippoventionSection { get; init; }
    public HippoventionCenterLocalizationDto? HippoventionCenterSection { get; init; }
    public GalleryLocalizationDto? AdvantagesSection { get; init; }
    public TextLocalizationDto? AnalysisSection { get; init; }
    public ScientificReferencesLocalizationDto? ScientificReferencesSection { get; init; }
    public QuoteLocalizationDto? AnotherQuoteSection { get; init; }
    public GalleryLocalizationDto? ParticipantsSection { get; init; }
    public EthicsLocalizationDto? EthicsSection { get; init; }
}

public record TextLocalizationDto(string Title, string Description);

public record QuoteLocalizationDto(string QuoteText, string? AuthorName);

public record HippoventionCenterLocalizationDto(string Title, string Description, string Pros);

public record CardLocalizationDto(long CardId, string Description);

public record GalleryLocalizationDto(string Title, List<CardLocalizationDto> Cards);

public record ReferenceLocalizationDto(long ReferenceId, string Name);

public record ScientificReferencesLocalizationDto(
    string Title, string Description, List<ReferenceLocalizationDto> References);

public record PrincipleLocalizationDto(long PrincipleId, string Text);

public record EthicsLocalizationDto(
    string Title, string Description, List<PrincipleLocalizationDto> Principles);

public record HippotherapyLandingPageTranslationStatusDto(
    HippotherapyLandingPageBlock Block, long LanguageId, TranslationStatus? TranslationStatus);
