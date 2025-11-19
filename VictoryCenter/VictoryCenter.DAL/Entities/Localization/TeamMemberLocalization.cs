namespace VictoryCenter.DAL.Entities.Localization;

public class TeamMemberLocalization : LocalizationBase<TeamMember>
{
    public string FullName { get; set; } = null!;

    public string? Description { get; set; }
}
