using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Entities.Interfaces;

namespace VictoryCenter.DAL.Entities;

public class CompanyProfileContact : BaseEntity, ITranslatedEntity<CompanyProfileContactLocalization>
{
    public long ProfileId { get; set; }
    public CompanyProfile Profile { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string CorrespondenceEmail { get; set; } = null!;
    public string Motto { get; set; } = null!;
    public ICollection<CompanyProfileContactLocalization> Localizations { get; set; } = [];
}
