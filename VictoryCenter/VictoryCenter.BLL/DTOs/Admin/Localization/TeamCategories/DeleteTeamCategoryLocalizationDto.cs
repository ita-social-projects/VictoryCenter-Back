using VictoryCenter.BLL.DTOs.Admin.Localization.Base;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.TeamCategories;
public class DeleteTeamCategoryLocalizationDto : ILocalizationIdentity
{
    public long EntityId { get; init; }

    public long LanguageId { get; init; }
}
