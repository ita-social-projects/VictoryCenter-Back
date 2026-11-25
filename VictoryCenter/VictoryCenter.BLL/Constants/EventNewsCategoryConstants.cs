namespace VictoryCenter.BLL.Constants;

public static class EventNewsCategoryConstants
{
    public static readonly int MinNameLength = 2;
    public static readonly int MaxNameLength = 20;

    public static readonly string DuplicateCategoryName = "An event/news category with this name already exists";
    public static readonly string CantDeleteCategoryWhileAssociatedWithEventNews =
        "Can't delete category while it is associated with any event/news item";

    public static readonly string DuplicateLocalizedName =
        "An event/news category localization with this name already exists for the selected language";
    public static readonly string LocalizationAlreadyExists =
        "A localization for this event/news category and language already exists";
}
