using System.Linq.Expressions;

namespace VictoryCenter.BLL.Services.Search.Helpers;

/// <summary>
/// Represents a single search term used to construct dynamic filtering expressions
/// for entities of type <typeparamref name="TEntity"/>.
/// A search term may correspond to a distinct property or a token within the same property,
/// depending on how the search logic is applied.
/// </summary>
/// <typeparam name="TEntity">The entity type being searched.</typeparam>
public class SearchTerm<TEntity>
{
    /// <summary>
    /// Gets or sets an expression that selects the string property of <typeparamref name="TEntity"/>
    /// to which the search term should be applied.
    /// </summary>
    /// <value>
    /// <placeholder>An expression that selects the string property of <typeparamref name="TEntity"/>
    /// to which the search term should be applied.</placeholder>
    /// </value>
    public Expression<Func<TEntity, string>> TermSelector { get; set; } = null!;

    /// <summary>
    /// Gets or sets the value to be searched for. If null or empty, this term is ignored.
    /// </summary>
    /// <value>
    /// <placeholder>The value to be searched for. If null or empty, this term is ignored.</placeholder>
    /// </value>
    public string? TermValue { get; set; }

    /// <summary>
    /// Gets or sets defines the type of search logic to apply, such as exact match or prefix match.
    /// </summary>
    /// <value>
    /// <placeholder>Defines the type of search logic to apply, such as exact match or prefix match.</placeholder>
    /// </value>
    public SearchLogic SearchLogic { get; set; }
}
