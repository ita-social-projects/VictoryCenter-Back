namespace VictoryCenter.DAL.Entities.Localization;
public class TeamCategoryLocalization : LocalizationBase<TeamCategory>
{
    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;
}
