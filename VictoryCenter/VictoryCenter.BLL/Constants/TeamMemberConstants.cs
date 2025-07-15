namespace VictoryCenter.BLL.Constants;

public static class TeamMemberConstants
{
    public const string FailedToCreateNewTeamMember = "Failed to create new TeamMember";
    public const string FailedToCreateNewTeamMemberInTheDatabase = "Failed to create new team member in database:";
    public const string FailedToDeleteTeamMember = "Failed to delete team member";
    public const string CategoryNotFoundOrContainsNoTeamMembers = "Category not found or contains no team members";
    public const string FailedToUpdateTeamMember = "Failed to update team member";
    public const string UnknownStatusValue = "Unknown status value";
    public const string OrderedIdsCannotBeEmpty = "Ordered Ids cannot be empty";
    public const string OrderedIdsMustContainUniqueValues = "OrderedIds must contain unique values";

    public static string OrderedIdsCannotContainMoreThanNElements(int numberOfElements)
    {
        return $"OrderedIds cannot contain more than {numberOfElements} elements";
    }

    public static string InvalidTeamMemberIdsFound(IEnumerable<long> ids)
    {
        return $"Invalid member IDs found: {string.Join(", ", ids)}";
    }
}
