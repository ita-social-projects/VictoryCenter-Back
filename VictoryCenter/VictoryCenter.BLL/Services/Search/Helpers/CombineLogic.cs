namespace VictoryCenter.BLL.Services.Search.Helpers;

/// <summary>
/// Specifies the logical operator used to combine multiple search terms.
/// </summary>
public enum CombineLogic
{
    /// <summary>
    /// All search conditions must be met (AND semantics).
    /// </summary>
    And,

    /// <summary>
    /// At least one search condition must be met (OR semantics).
    /// </summary>
    Or,
}
