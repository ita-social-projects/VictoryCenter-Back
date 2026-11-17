using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Entities;

[Auditable]
public class TeamCategory : BaseEntity, ITranslatedEntity<TeamCategoryLocalization>
{
    [Audited]
    public string Name { get; set; } = null!;

    [Audited]
    public string Description { get; set; } = null!;

    public ICollection<TeamMember> TeamMembers { get; set; } = [];

    public ICollection<TeamCategoryLocalization> Localizations { get; set; } = [];
}
