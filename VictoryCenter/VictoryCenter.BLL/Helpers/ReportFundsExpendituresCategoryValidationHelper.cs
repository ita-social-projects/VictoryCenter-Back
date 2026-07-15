using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Helpers;

public static class ReportFundsExpendituresCategoryValidationHelper
{
    public static bool IsReservedCategoryName(string name, ReportFundsExpendituresType type)
    {
        return type == ReportFundsExpendituresType.Expense &&
            name.Contains(
                ReportFundsExpendituresCategoryConstants.ReservedCategoryNamePrefix,
                StringComparison.OrdinalIgnoreCase);
    }
}
