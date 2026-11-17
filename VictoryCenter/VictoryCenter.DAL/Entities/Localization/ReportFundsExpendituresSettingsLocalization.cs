using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities.Localization;

public class ReportFundsExpendituresSettingsLocalization : LocalizationBase<ReportFundsExpendituresSettings>
{
    [Audited]
    public string DisclaimerTitle { get; set; } = null!;
}
