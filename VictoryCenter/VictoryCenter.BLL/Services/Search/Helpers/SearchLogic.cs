namespace VictoryCenter.BLL.Services.Search.Helpers;

// More different types of search can be added here

/// <summary>
/// Specifies the type of search logic to apply when evaluating a search term.
/// </summary>
public enum SearchLogic
{
    /// <summary>
    /// Matches values that are exactly equal to the search term.
    /// </summary>
    Exact,

    /// <summary>
    /// Matches values that start with the specific value
    /// </summary>
    Prefix,
}
