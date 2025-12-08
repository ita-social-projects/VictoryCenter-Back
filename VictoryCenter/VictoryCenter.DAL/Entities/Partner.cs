using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Interfaces;

namespace VictoryCenter.DAL.Entities;

public class Partner : BaseEntity, IOrderableEntity
{
    public long PartnersSectionId { get; set; }
    public string Description { get; set; } = null!;
    public long Priority { get; set; }
    public long ImageId { get; set; }
    public Image Image { get; set; } = null!;
    public PartnerSection PartnerSection { get; set; } = null!;
}
