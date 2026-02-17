using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities.WhoWeAreContents;

public abstract class WhoWeAreContent : ITranslatedEntity<WhoWeAreContentLocalization>, IEntity
{
    public long Id { get; set; }

    public long SectionId { get; set; }

    public ContentType ContentType { get; set; }

    public WhoWeAreSection Section { get; set; } = null!;

    public ICollection<WhoWeAreContentLocalization> Localizations { get; set; } = [];
}
