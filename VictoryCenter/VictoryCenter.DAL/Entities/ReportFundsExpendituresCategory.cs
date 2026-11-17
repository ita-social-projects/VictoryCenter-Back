using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

[Auditable]
public class ReportFundsExpendituresCategory : BaseEntity, ITranslatedEntity<ReportFundsExpendituresCategoryLocalization>
{
    [Audited]
    public string Name { get; set; } = "";

    [Audited]
    public ReportFundsExpendituresType Type { get; set; }

    public ICollection<ReportFundsExpendituresRecord> Records { get; set; } = [];
    public ICollection<ReportFundsExpendituresCategoryLocalization> Localizations { get; set; } = [];
}
