using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

[Auditable]
public class HippotherapyLandingPageQuoteSection
    : BaseEntity, ITranslatedEntity<HippotherapyLandingPageQuoteSectionLocalization>
{
    [Audited]
    public long HippotherapyLandingPageId { get; set; }

    [Audited]
    [RichText]
    public string QuoteText { get; set; } = null!;

    [Audited]
    [RichText]
    public string? AuthorName { get; set; }

    [Audited]
    public long? ImageId { get; set; }

    public Image? Image { get; set; }

    public HippotherapyLandingPage HippotherapyLandingPage { get; set; } = null!;

    public ICollection<HippotherapyLandingPageQuoteSectionLocalization> Localizations { get; set; } = [];
}
