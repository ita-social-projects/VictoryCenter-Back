namespace VictoryCenter.BLL.Constants;

public static class ReportFundsExpendituresCategoryConstants
{
    public static readonly int NameMaxLength = 255;
    public static readonly string DuplicateCategoryName = "Category with the same name already exists";

    public static readonly string CantDeleteCategoryWhileAssociatedWithAnyRecord =
        "Can't delete category while associated with any funds and expenditures record";

    public static readonly string ReservedCategoryNamePrefix = "Програмні";

    public static readonly string CantDeleteReservedCategory = "Can't delete a reserved category";

    public static readonly string CantUpdateReservedCategory = "Can't update a reserved category";

    public static readonly string ReservedCategoryName = "This category name is reserved";
}
