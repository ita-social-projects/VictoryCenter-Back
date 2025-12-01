namespace VictoryCenter.BLL.Constants;

public static class TeamMemberConstants
{
    // Allows international names containing letters, spaces, hyphens, and apostrophes.
    public static readonly string FullNameRegexPattern = @"^[\p{L}'\u2019\- ]+$";
    public static readonly int FullNameMinLength = 2;
    public static readonly int FullNameMaxLength = 100;
    public static readonly int DescriptionNameMinLength = 10;
    public static readonly int DescriptionNameMaxLength = 200;

    public static readonly string CategoryNotFoundOrContainsNoTeamMembers = "Category not found or contains no team members";
    public static readonly string FailedRetrievingMemberPhoto = "Failed to retrieve team member photo ";
}
