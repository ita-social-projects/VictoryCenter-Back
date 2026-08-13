namespace VictoryCenter.BLL.Constants;

public static class TeamCategoryConstants
{
    public static readonly string CantDeleteCategoryWhileAssociatedWithAnyTeamMember =
        "Can't delete category while associated with any team member";
    public static readonly string DuplicateCategoryName = "Category with the same name already exists";
    public static readonly int MinNameLength = 5;
    public static readonly int MaxNameLength = 20;
    public static readonly int MinDescriptionLength = 5;
    public static readonly int MaxDescriptionLength = 200;
}
