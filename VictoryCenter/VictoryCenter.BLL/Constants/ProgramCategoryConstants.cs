namespace VictoryCenter.BLL.Constants;

public class ProgramCategoryConstants
{
    public static readonly string FailedToCreateCategory = "Failed to create category";
    public static readonly string CantDeleteProgramCategoryWhileAssociatedWithAnyProgram =
        "Can't delete category while associated with any program";
    public static readonly string FailedToDeleteCategory = "Failed to delete category";
    public static readonly string FailedToUpdateCategory = "Failed to update category";
    public static readonly int MaxNameLength = 20;
    public static readonly int MinNameLength = 5;
}
