namespace VictoryCenter.DAL.Entities;

public class ProgramCategory
{
    public long Id { get; set; }
    public string Name { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public ICollection<Program> Programs { get; set; } = new List<Program>();
}
