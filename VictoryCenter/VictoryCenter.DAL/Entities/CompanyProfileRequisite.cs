using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Entities;

public class CompanyProfileRequisite : BaseEntity, ITranslatedEntity<CompanyProfileRequisiteLocalization>
{
    public long ProfileId { get; set; }
    public CompanyProfile Profile { get; set; } = null!;
    public string Recipient { get; set; } = null!;
    public string Edrpou { get; set; } = null!;
    public string Address { get; set; } = null!;
    public ICollection<CompanyProfileRequisiteLocalization> Localizations { get; set; } = [];
}
