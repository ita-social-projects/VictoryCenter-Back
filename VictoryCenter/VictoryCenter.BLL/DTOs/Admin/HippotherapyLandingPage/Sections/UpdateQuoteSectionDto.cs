namespace VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage.Sections;

public record UpdateQuoteSectionDto
{
    public string QuoteText { get; init; } = null!;

    public string? AuthorName { get; init; }

    public long? ImageId { get; init; }
}
