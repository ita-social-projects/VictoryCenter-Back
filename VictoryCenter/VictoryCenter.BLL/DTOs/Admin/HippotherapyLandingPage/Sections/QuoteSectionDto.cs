using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage.Sections;

public record QuoteSectionDto
{
    public string QuoteText { get; init; } = null!;

    public string? AuthorName { get; init; }

    public ImageDto? Image { get; init; }
}
