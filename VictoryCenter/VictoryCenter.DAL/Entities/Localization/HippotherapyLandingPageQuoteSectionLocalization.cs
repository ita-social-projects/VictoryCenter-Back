using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

namespace VictoryCenter.DAL.Entities.Localization;

public class HippotherapyLandingPageQuoteSectionLocalization : LocalizationBase<HippotherapyLandingPageQuoteSection>
{
    [Audited]
    [RichText]
    public string QuoteText { get; set; } = null!;

    [Audited]
    [RichText]
    public string? AuthorName { get; set; }
}
