using VictoryCenter.DAL.Entities.Interfaces;

namespace VictoryCenter.DAL.Entities;

public class PartnerSection : IOrderableEntity
{
    public long Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public long Priority { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<Partner> Partners { get; set; } = [];
}
