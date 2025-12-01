using VictoryCenter.DAL.Data.BaseEntity;

namespace VictoryCenter.DAL.Entities;

public class VisitorPage : BaseEntity
{
    public string Slug { get; set; } = null!;

    public string Title { get; set; } = null!;

    public ICollection<FaqPlacement> Questions { get; set; } = [];
}
