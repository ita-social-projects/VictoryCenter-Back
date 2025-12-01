using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Interfaces;

namespace VictoryCenter.DAL.Entities;

public class PartnerSection : BaseEntity, IOrderableEntity
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public long Priority { get; set; }
    public ICollection<Partner> Partners { get; set; } = [];
}
