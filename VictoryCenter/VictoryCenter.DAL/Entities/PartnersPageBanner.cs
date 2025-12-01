using VictoryCenter.DAL.Data.BaseEntity;

namespace VictoryCenter.DAL.Entities;

public class PartnersPageBanner : BaseEntity
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public long? ImageId { get; set; }
    public Image? Image { get; set; }
}
