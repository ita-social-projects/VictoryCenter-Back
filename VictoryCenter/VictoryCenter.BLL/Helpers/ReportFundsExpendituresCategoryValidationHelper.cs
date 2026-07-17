using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Helpers;

public static class ReportFundsExpendituresCategoryValidationHelper
{
    public static bool IsReservedCategoryName(string name, ReportFundsExpendituresType type)
    {
        if (string.IsNullOrEmpty(name))
        {
            return false;
        }

        return type == ReportFundsExpendituresType.Expense &&
            name.Trim().StartsWith(
                ReportFundsExpendituresCategoryConstants.ReservedCategoryNamePrefix,
                StringComparison.OrdinalIgnoreCase);
    }
}
