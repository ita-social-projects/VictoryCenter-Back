using VictoryCenter.BLL.Helpers;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.HelperTests;

public class ReportFundsExpendituresCategoryValidationHelperTests
{
    [Fact]
    public void IsReservedCategoryName_ExactReservedNameAndExpenseType_ReturnsTrue()
    {
        var result = ReportFundsExpendituresCategoryValidationHelper.IsReservedCategoryName(
            "Програмні", ReportFundsExpendituresType.Expense);

        Assert.True(result);
    }

    [Theory]
    [InlineData("ПРОГРАМНІ")]
    [InlineData("програмні тест")]
    [InlineData("Програмні тест 2")]
    public void IsReservedCategoryName_RealWorldNameVariant_ReturnsTrue(string name)
    {
        var result = ReportFundsExpendituresCategoryValidationHelper.IsReservedCategoryName(
            name, ReportFundsExpendituresType.Expense);

        Assert.True(result);
    }

    [Fact]
    public void IsReservedCategoryName_IncomeType_ReturnsFalse()
    {
        var result = ReportFundsExpendituresCategoryValidationHelper.IsReservedCategoryName(
            "Програмні тест 2", ReportFundsExpendituresType.Income);

        Assert.False(result);
    }

    [Fact]
    public void IsReservedCategoryName_UnrelatedName_ReturnsFalse()
    {
        var result = ReportFundsExpendituresCategoryValidationHelper.IsReservedCategoryName(
            "Оренда офісу", ReportFundsExpendituresType.Expense);

        Assert.False(result);
    }
}
