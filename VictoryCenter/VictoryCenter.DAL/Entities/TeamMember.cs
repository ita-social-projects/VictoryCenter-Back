using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

[Auditable]
public class TeamMember : BaseEntity, IOrderableEntity, ITranslatedEntity<TeamMemberLocalization>
{
    [Audited]
    public string FullName { get; set; } = null!;

    [Audited]
    public long CategoryId { get; set; }

    [Audited]
    public long Priority { get; set; }

    [Audited]
    public Status Status { get; set; }

    [Audited]
    public string? Description { get; set; }

    [Audited]
    public long? ImageId { get; set; }

    public string? Email { get; set; }

    public TeamCategory TeamCategory { get; set; } = null!;

    public Image? Image { get; set; }

    public ICollection<TeamMemberLocalization> Localizations { get; set; } = [];
}
