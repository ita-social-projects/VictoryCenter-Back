using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramCategories;

public class HippotherapyProgramCategoryLocalizationDto
{
    public long EntityId { get; init; }
    public LocalizationInfoDto LocalizationInfoDto { get; init; } = null!;
    public string Name { get; set; } = null!;
    public TranslationStatus TranslationStatus { get; init; }
}
