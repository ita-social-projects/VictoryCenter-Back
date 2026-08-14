using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Entities;

public class ReportFundsExpendituresSettings : BaseEntity, ITranslatedEntity<ReportFundsExpendituresSettingsLocalization>
{
    public string DisclaimerTitle { get; set; } = "";
    public decimal ExchangeRate { get; set; }
    public int ProgramExpendituresReportingYear { get; set; }
    public bool HasUnpublishedChanges { get; set; }

    public ICollection<ReportFundsExpendituresSettingsLocalization> Localizations { get; set; } = [];
}
