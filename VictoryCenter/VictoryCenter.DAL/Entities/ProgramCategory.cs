using VictoryCenter.DAL.Data.BaseEntity;

namespace VictoryCenter.DAL.Entities;

public class ProgramCategory : BaseEntity
{
    public string Name { get; set; } = null!;
    public ICollection<Program> Programs { get; set; } = [];
}
