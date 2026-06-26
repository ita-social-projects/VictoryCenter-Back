using VictoryCenter.DAL.Data.BaseEntity;

namespace VictoryCenter.DAL.Entities;
public class CollectedFundsBlock : BaseEntity
{
    public string Title { get; set; }
    public string TitleEn { get; set; }
    public long? ImageId { get; set; }
    public Image? Image { get; set; } = null;
}
