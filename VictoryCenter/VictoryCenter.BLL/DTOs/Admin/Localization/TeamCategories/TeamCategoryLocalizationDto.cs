using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.TeamCategories;
public class TeamCategoryLocalizationDto
{
    public long EntityId { get; init; }
    public LocalizationInfoDto LocalizationInfoDto { get; init; } = null!;
    public string FullName { get; set; } = null!;

    public string? Description { get; set; }

    public TranslationStatus TranslationStatus { get; init; }
}
