using VictoryCenter.DAL.Data.BaseEntity;

namespace VictoryCenter.DAL.Entities;
public class ChangedLivesBlock : BaseEntity
{
    public string Title { get; set; }
    public string TitleEn { get; set; }
    public int ChangedLivesCount { get; set; }
    public long? ImageId { get; set; }
    public Image? Image { get; set; } = null;
}
