using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage.Sections;

namespace VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage;

public record HippotherapyLandingPageDto
{
    public IntroSectionDto IntroSection { get; init; } = null!;

    public TextSectionDto DescriptionSection { get; init; } = null!;

    public QuoteSectionDto QuoteSection { get; init; } = null!;

    public TextSectionDto HippoventionSection { get; init; } = null!;

    public HippoventionCenterSectionDto HippoventionCenterSection { get; init; } = null!;

    public GallerySectionDto AdvantagesSection { get; init; } = null!;

    public TextSectionDto AnalysisSection { get; init; } = null!;

    public ScientificReferencesSectionDto ScientificReferencesSection { get; init; } = null!;

    public QuoteSectionDto AnotherQuoteSection { get; init; } = null!;

    public GallerySectionDto ParticipantsSection { get; init; } = null!;

    public EthicsSectionDto EthicsSection { get; init; } = null!;
}
