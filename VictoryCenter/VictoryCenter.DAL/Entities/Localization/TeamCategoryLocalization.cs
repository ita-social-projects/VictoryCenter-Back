namespace VictoryCenter.DAL.Entities.Localization;
public class TeamCategoryLocalization : LocalizationBase<TeamCategory>
{
    public string FullName { get; set; } = null!;

    public string? Description { get; set; }
}
