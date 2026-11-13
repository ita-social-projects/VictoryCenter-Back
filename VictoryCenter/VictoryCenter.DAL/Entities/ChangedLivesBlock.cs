using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities;

[Auditable]
public class ChangedLivesBlock : BaseEntity
{
    [Audited]
    public string Title { get; set; }

    [Audited]
    public string TitleEn { get; set; }

    [Audited]
    public int ChangedLivesCount { get; set; }

    [Audited]
    public long? ImageId { get; set; }

    public Image? Image { get; set; } = null;
}
