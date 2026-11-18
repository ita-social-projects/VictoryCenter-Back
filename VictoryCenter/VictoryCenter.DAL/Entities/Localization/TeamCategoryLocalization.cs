using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities.Localization;

public class TeamCategoryLocalization : LocalizationBase<TeamCategory>
{
    [Audited]
    public string Name { get; set; } = null!;

    [Audited]
    public string Description { get; set; } = null!;
}
