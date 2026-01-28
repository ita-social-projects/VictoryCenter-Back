using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Entities;

public class TeamCategory : BaseEntity, ITranslatedEntity<TeamCategoryLocalization>
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public ICollection<TeamMember> TeamMembers { get; set; } = [];
    public ICollection<TeamCategoryLocalization> Localizations { get; set; } = [];
}
