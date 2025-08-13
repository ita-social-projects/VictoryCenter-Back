using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

public class SupportMethod
{
    public long Id { get; set; }
    public Currency Currency { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<AdditionalField> AdditionalFields { get; set; } = new List<AdditionalField>();
}
