using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageQuoteSection;

public class HippotherapyLandingPageQuoteSectionLocalizationDto
{
    public long EntityId { get; init; }

    public LocalizationInfoDto LocalizationInfoDto { get; init; } = null!;

    public string QuoteText { get; set; } = null!;

    public string? AuthorName { get; set; }

    public TranslationStatus TranslationStatus { get; init; }
}
