using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage.Sections;

namespace VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage;

public record UpdateHippotherapyLandingPageDto
{
    public UpdateIntroSectionDto IntroSection { get; init; } = null!;

    public UpdateTextSectionDto DescriptionSection { get; init; } = null!;

    public UpdateQuoteSectionDto QuoteSection { get; init; } = null!;

    public UpdateTextSectionDto HippoventionSection { get; init; } = null!;

    public UpdateHippoventionCenterSectionDto HippoventionCenterSection { get; init; } = null!;

    public UpdateGallerySectionDto AdvantagesSection { get; init; } = null!;

    public UpdateTextSectionDto AnalysisSection { get; init; } = null!;

    public UpdateScientificReferencesSectionDto ScientificReferencesSection { get; init; } = null!;

    public UpdateQuoteSectionDto AnotherQuoteSection { get; init; } = null!;

    public UpdateGallerySectionDto ParticipantsSection { get; init; } = null!;

    public UpdateEthicsSectionDto EthicsSection { get; init; } = null!;
}
