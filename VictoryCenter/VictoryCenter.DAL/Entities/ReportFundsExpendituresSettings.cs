using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Entities;

[Auditable]
public class ReportFundsExpendituresSettings : BaseEntity, ITranslatedEntity<ReportFundsExpendituresSettingsLocalization>
{
    [Audited]
    public string DisclaimerTitle { get; set; } = "";

    [Audited]
    public decimal ExchangeRate { get; set; }

    [Audited]
    public int ProgramExpendituresReportingYear { get; set; }

    [Audited]
    public bool HasUnpublishedChanges { get; set; }

    public ICollection<ReportFundsExpendituresSettingsLocalization> Localizations { get; set; } = [];
}
