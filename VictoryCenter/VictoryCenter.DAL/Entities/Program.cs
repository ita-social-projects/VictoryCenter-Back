namespace VictoryCenter.DAL.Entities;

public class Program
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
#pragma warning disable SA1011
    public byte[]? Photo { get; set; }
#pragma warning restore SA1011
    public ICollection<ProgramCategory> Categories { get; set; } = new List<ProgramCategory>();
}
