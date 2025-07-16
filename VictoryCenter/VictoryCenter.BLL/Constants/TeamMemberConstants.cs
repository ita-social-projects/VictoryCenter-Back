namespace VictoryCenter.BLL.Constants;

public static class TeamMemberConstants
{
    public static readonly string FailedToCreateNewTeamMember = "Failed to create new TeamMember";
    public static readonly string FailedToCreateNewTeamMemberInTheDatabase = "Failed to create new team member in database:";
    public static readonly string FailedToDeleteTeamMember = "Failed to delete team member";
    public static readonly string CategoryNotFoundOrContainsNoTeamMembers = "Category not found or contains no team members";
    public static readonly string FailedToUpdateTeamMember = "Failed to update team member";
    public static readonly string UnknownStatusValue = "Unknown status value";
    public static readonly string OrderedIdsCannotBeEmpty = "Ordered Ids cannot be empty";
    public static readonly string OrderedIdsMustContainUniqueValues = "OrderedIds must contain unique values";

    public static string OrderedIdsCannotContainMoreThanNElements(int numberOfElements)
    {
        return $"OrderedIds cannot contain more than {numberOfElements} elements";
    }

    public static string InvalidTeamMemberIdsFound(IEnumerable<long> ids)
    {
        return $"Invalid member IDs found: {string.Join(", ", ids)}";
    }
}
