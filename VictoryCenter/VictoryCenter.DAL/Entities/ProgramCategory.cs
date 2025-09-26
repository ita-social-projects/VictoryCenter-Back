using VictoryCenter.DAL.Data.BaseEntity;

namespace VictoryCenter.DAL.Entities;

public class ProgramCategory : BaseEntity
{
    public string Name { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
    public ICollection<Program> Programs { get; set; } = new List<Program>();
}
