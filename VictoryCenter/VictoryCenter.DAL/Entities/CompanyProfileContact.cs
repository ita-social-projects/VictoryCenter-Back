using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Entities;

[Auditable]
public class CompanyProfileContact : BaseEntity, ITranslatedEntity<CompanyProfileContactLocalization>
{
    [Audited]
    public long ProfileId { get; set; }

    public CompanyProfile Profile { get; set; } = null!;

    [Audited]
    public string Phone { get; set; } = null!;

    [Audited]
    public string Address { get; set; } = null!;

    [Audited]
    public string Email { get; set; } = null!;

    [Audited]
    public string CorrespondenceEmail { get; set; } = null!;

    [Audited]
    public string Motto { get; set; } = null!;

    public ICollection<CompanyProfileContactLocalization> Localizations { get; set; } = [];
}
