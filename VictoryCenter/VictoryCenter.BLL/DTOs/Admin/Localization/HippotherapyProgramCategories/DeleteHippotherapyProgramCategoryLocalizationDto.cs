using VictoryCenter.BLL.DTOs.Admin.Localization.Base;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramCategories;

public record DeleteHippotherapyProgramCategoryLocalizationDto : ILocalizationIdentity
{
    public long EntityId { get; init; }

    public long LanguageId { get; init; }
}
