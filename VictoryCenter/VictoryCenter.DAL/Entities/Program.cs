using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

public class Program
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public Status Status { get; set; }
    public long? ImageId { get; set; }
    public ICollection<ProgramCategory> Categories { get; set; } = new List<ProgramCategory>();
    public Image? Image { get; set; }
}
