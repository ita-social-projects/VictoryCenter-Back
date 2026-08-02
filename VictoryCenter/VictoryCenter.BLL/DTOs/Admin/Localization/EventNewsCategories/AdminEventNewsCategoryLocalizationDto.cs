using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.EventNewsCategories;

public record AdminEventNewsCategoryLocalizationDto
{
    public long EntityId { get; init; }
    public LocalizationInfoDto Language { get; init; } = null!;
    public string Name { get; init; } = null!;
    public TranslationStatus TranslationStatus { get; init; }
}
