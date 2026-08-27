using VictoryCenter.DAL.Data.BaseEntity;

namespace VictoryCenter.DAL.Entities;

public class FeedbackHistory : BaseEntity
{
    public string Title { get; set; } = null!;
    public string Story { get; set; } = null!;
    public long? ImageId { get; set; }
    public Image? Image { get; set; }
}
