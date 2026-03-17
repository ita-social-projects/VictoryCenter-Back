namespace VictoryCenter.BLL.Constants;

public static class ReportFundsExpendituresCategoryConstants
{
    public static readonly int MaxCategoriesCountPerType = 4;
    public static readonly int NameMaxLength = 255;
    public static readonly string DuplicateCategoryName = "Category with the same name already exists";

    public static readonly string CannotCreateCategoryWhenMaximumCountReached =
        $"Cannot create category because the maximum number of categories ({MaxCategoriesCountPerType}) for this type has been reached";

    public static readonly string CantDeleteCategoryWhileAssociatedWithAnyRecord =
        "Can't delete category while associated with any funds and expenditures record";
}
