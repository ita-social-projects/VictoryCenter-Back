using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.History;

public record HistorySectionContentLocalizationDto
{
    public long EntityId { get; init; }
    public long LanguageId { get; init; }
    public LocalizationInfoDto LocalizationInfoDto { get; init; } = null!;
    public string? Title { get; init; }
    public string? Description { get; init; }
    public TranslationStatus TranslationStatus { get; init; }
}
