namespace VictoryCenter.DAL.Entities;

public class ProgramCategory
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
    public ICollection<Program> Programs { get; set; } = [];
}
