using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

public class HippotherapyLandingPageQuoteSection
    : BaseEntity, ITranslatedEntity<HippotherapyLandingPageQuoteSectionLocalization>
{
    public long HippotherapyLandingPageId { get; set; }

    public string QuoteText { get; set; } = null!;

    public string? AuthorName { get; set; }

    public long? ImageId { get; set; }

    public Image? Image { get; set; }

    public HippotherapyLandingPage HippotherapyLandingPage { get; set; } = null!;

    public ICollection<HippotherapyLandingPageQuoteSectionLocalization> Localizations { get; set; } = [];
}
