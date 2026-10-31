using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

namespace VictoryCenter.DAL.Entities.Localization;

public class HippotherapyLandingPageQuoteSectionLocalization : LocalizationBase<HippotherapyLandingPageQuoteSection>
{
    [RichText]
    public string QuoteText { get; set; } = null!;

    [RichText]
    public string? AuthorName { get; set; }
}
