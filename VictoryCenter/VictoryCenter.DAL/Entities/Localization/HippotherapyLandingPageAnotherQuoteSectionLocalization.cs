using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

namespace VictoryCenter.DAL.Entities.Localization;

public class HippotherapyLandingPageAnotherQuoteSectionLocalization : LocalizationBase<HippotherapyLandingPageAnotherQuoteSection>
{
    public string QuoteText { get; set; } = null!;

    public string? AuthorName { get; set; }
}
