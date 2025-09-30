using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

public class HypotherapyProgram : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public Status Status { get; set; }
    public long? ImageId { get; set; }
    public ICollection<ProgramCategory> Categories { get; set; } = [];
    public Image? Image { get; set; }
}
