namespace VictoryCenter.BLL.Constants;

public static class EventNewsCategoryConstants
{
    public const int MinNameLength = 2;
    public const int MaxNameLength = 20;

    public const string DuplicateCategoryName = "An event/news category with this name already exists";
    public const string DuplicateLocalizedName =
        "An event/news category localization with this name already exists for the selected language";
    public const string LocalizationAlreadyExists =
        "A localization for this event/news category and language already exists";
    public const string CantDeleteCategoryWhileAssociatedWithEventNews =
        "Can't delete category while it is associated with any event/news item";
}
