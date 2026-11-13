using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Entities;

[Auditable]
public class CompanyProfileRequisite : BaseEntity, ITranslatedEntity<CompanyProfileRequisiteLocalization>
{
    [Audited]
    public long ProfileId { get; set; }

    public CompanyProfile Profile { get; set; } = null!;

    [Audited]
    public string Recipient { get; set; } = null!;

    public string Edrpou { get; set; } = null!;

    [Audited]
    public string Address { get; set; } = null!;

    public ICollection<CompanyProfileRequisiteLocalization> Localizations { get; set; } = [];
}
