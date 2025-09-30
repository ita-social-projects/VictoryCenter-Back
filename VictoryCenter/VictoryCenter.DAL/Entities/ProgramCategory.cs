using VictoryCenter.DAL.Data.BaseEntity;

namespace VictoryCenter.DAL.Entities;

public class ProgramCategory : BaseEntity
{
    public string Name { get; set; } = null!;
    public ICollection<HypotherapyProgram> Programs { get; set; } = new List<HypotherapyProgram>();
}
