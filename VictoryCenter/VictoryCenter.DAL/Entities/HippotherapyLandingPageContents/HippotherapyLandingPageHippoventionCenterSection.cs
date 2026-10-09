using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

public class HippotherapyLandingPageHippoventionCenterSection
    : BaseEntity, ITranslatedEntity<HippotherapyLandingPageHippoventionCenterSectionLocalization>
{
    public long HippotherapyLandingPageId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public long? ImageId { get; set; }

    public Image? Image { get; set; }

    public string Pros { get; set; } = null!;

    public HippotherapyLandingPage HippotherapyLandingPage { get; set; } = null!;

    public ICollection<HippotherapyLandingPageHippoventionCenterSectionLocalization> Localizations { get; set; } = [];
}
