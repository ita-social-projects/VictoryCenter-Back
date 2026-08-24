using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

namespace VictoryCenter.DAL.Entities;

public class HippotherapyLandingPage : BaseEntity
{
    public HippotherapyLandingPageIntroSection? IntroSection { get; set; }

    public HippotherapyLandingPageDescriptionSection? DescriptionSection { get; set; }

    public HippotherapyLandingPageQuoteSection? QuoteSection { get; set; }

    public HippotherapyLandingPageHippoventionSection? HippoventionSection { get; set; }

    public HippotherapyLandingPageHippoventionCenterSection? HippoventionCenterSection { get; set; }

    public HippotherapyLandingPageAdvantagesSection? AdvantagesSection { get; set; }

    public HippotherapyLandingPageAnalysisSection? AnalysisSection { get; set; }

    public HippotherapyLandingPageScientificReferencesSection? ScientificReferencesSection { get; set; }

    public HippotherapyLandingPageAnotherQuoteSection? AnotherQuoteSection { get; set; }

    public HippotherapyLandingPageParticipantsSection? ParticipantsSection { get; set; }

    public HippotherapyLandingPageEthicsSection? EthicsSection { get; set; }
}
