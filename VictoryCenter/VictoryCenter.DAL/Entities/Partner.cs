using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Entities;

public class Partner : BaseEntity, IOrderableEntity, ITranslatedEntity<PartnerLocalization>
{
    public long PartnersSectionId { get; set; }
    public string Description { get; set; } = null!;
    public long Priority { get; set; }
    public long ImageId { get; set; }
    public Image Image { get; set; } = null!;
    public PartnerSection PartnerSection { get; set; } = null!;
    public ICollection<PartnerLocalization> Localizations { get; set; } = [];
}
