using VictoryCenter.DAL.Entities.Interfaces;

namespace VictoryCenter.DAL.Entities;

public class Partner : IOrderableEntity
{
    public long Id { get; set; }
    public long PartnersSectionId { get; set; }
    public string Description { get; set; } = null!;
    public long Priority { get; set; }
    public DateTime CreatedAt { get; set; }
    public long? ImageId { get; set; }
    public Image? Image { get; set; }
    public PartnerSection PartnerSection { get; set; } = null!;
}
