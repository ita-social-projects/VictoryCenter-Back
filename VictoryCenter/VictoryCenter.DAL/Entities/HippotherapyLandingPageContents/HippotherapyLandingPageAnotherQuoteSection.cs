using VictoryCenter.DAL.Data.BaseEntity;

namespace VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

public class HippotherapyLandingPageAnotherQuoteSection : BaseEntity
{
    public long HippotherapyLandingPageId { get; set; }

    public string QuoteText { get; set; } = null!;

    public string? AuthorName { get; set; }

    public long? ImageId { get; set; }

    public Image? Image { get; set; }

    public HippotherapyLandingPage HippotherapyLandingPage { get; set; } = null!;
}
