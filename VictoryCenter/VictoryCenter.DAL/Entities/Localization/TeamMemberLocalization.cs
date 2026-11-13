using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities.Localization;

public class TeamMemberLocalization : LocalizationBase<TeamMember>
{
    [Audited]
    public string FullName { get; set; } = null!;

    [Audited]
    public string? Description { get; set; }
}
