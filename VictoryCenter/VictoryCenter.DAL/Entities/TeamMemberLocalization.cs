namespace VictoryCenter.DAL.Entities;

public class TeamMemberLocalization : LocalizationBase<TeamMember>
{
    public string FullName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }
}
