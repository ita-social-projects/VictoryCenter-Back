using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities.Localization;

public class ReportFundsExpendituresCategoryLocalization : LocalizationBase<ReportFundsExpendituresCategory>
{
    [Audited]
    public string Name { get; set; } = null!;
}
