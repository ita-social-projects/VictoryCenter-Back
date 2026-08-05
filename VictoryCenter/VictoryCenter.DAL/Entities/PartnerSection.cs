using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Entities;

public class PartnerSection : BaseEntity, IOrderableEntity, ITranslatedEntity<PartnerSectionLocalization>
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public long Priority { get; set; }
    public ICollection<Partner> Partners { get; set; } = [];
    public ICollection<PartnerSectionLocalization> Localizations { get; set; } = [];
}
