using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.TeamCategories;
public class TeamCategoryLocalizationDto
{
    public long EntityId { get; init; }
    public LocalizationInfoDto LocalizationInfoDto { get; init; } = null!;
    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public TranslationStatus TranslationStatus { get; init; }
}
