namespace VictoryCenter.BLL.Constants;

public static class ReportFundsExpendituresCategoryConstants
{
    public static readonly int NameMaxLength = 255;
    public static readonly string DuplicateCategoryName = "Category with the same name already exists";

    public static readonly string CantDeleteCategoryWhileAssociatedWithAnyRecord =
        "Can't delete category while associated with any funds and expenditures record";
}
