using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

namespace VictoryCenter.DAL.Entities.Localization;

public class HippotherapyLandingPageQuoteSectionLocalization : LocalizationBase<HippotherapyLandingPageQuoteSection>
{
    public string QuoteText { get; set; } = null!;

    public string? AuthorName { get; set; }
}
