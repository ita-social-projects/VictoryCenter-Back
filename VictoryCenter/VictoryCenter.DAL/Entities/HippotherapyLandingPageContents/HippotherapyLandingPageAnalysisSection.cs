using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

public class HippotherapyLandingPageAnalysisSection
    : BaseEntity, ITranslatedEntity<HippotherapyLandingPageAnalysisSectionLocalization>
{
    public long HippotherapyLandingPageId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public HippotherapyLandingPage HippotherapyLandingPage { get; set; } = null!;

    public ICollection<HippotherapyLandingPageAnalysisSectionLocalization> Localizations { get; set; } = [];
}
