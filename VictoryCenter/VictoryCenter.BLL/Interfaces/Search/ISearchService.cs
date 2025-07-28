using System.Linq.Expressions;
using VictoryCenter.BLL.Services.Search.Helpers;

namespace VictoryCenter.BLL.Interfaces.Search;

/// <summary>
/// Defines a contract for building dynamic search expressions for entities of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the entity being searched. Must be a reference type.</typeparam>
public interface ISearchService<T>
    where T : class
{
    /// <summary>
    /// Builds an expression that represents a combination of search terms used to filter entities.
    /// </summary>
    /// <param name="searchTerms">
    /// One or more <see cref="SearchTerm{T}"/> instances that define which properties to search,
    /// the values to match, and the logic to use (e.g., exact match or prefix match).
    /// Terms with null or empty values are automatically ignored.
    /// </param>
    /// <returns>
    /// An expression of type <see cref="Expression{Func{T, bool}}"/> that can be used for filtering.
    /// Returns an expression that always evaluates to true if no valid terms are provided.
    /// </returns>
    Expression<Func<T, bool>> CreateSearchExpression(params SearchTerm<T>[] searchTerms);
}
