namespace VictoryCenter.DAL.Entities;

public class PartnersPageBanner
{
    public long Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
    public long ImageId { get; set; }
    public Image Image { get; set; } = null!;
}
