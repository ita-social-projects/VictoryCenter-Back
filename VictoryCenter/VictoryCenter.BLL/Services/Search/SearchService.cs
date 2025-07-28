using System.Linq.Expressions;
using VictoryCenter.BLL.Interfaces.Search;
using VictoryCenter.BLL.Services.Search.Helpers;

namespace VictoryCenter.BLL.Services.Search;

public class SearchService<T> : ISearchService<T>
    where T : class
{
    public SearchService()
    {
    }

    // Generic method to create any search expressions
    public Expression<Func<T, bool>> CreateSearchExpression(params SearchTerm<T>[] searchTerms)
    {
        // Represents the value e in e => e.Name
        var parameter = Expression.Parameter(typeof(T), "e");
        Expression? combined = null;

        foreach (var term in searchTerms)
        {
            // Skip empty values
            if (string.IsNullOrEmpty(term.TermValue))
            {
                continue;
            }

            // Replace the parameter in the term selector with the correct parameter a => a.Name ==> e => e.Name
            var member = new ReplaceParameterVisitor(term.TermSelector.Parameters[0], parameter)
                .Visit(term.TermSelector.Body);

            // Represents the value to search in the expression
            var constant = Expression.Constant(term.TermValue, typeof(string));

            Expression body;

            switch (term.SearchLogic)
            {
                case SearchLogic.Exact:
                    body = Expression.Equal(member, constant);
                    break;

                case SearchLogic.Prefix:
                    body = Expression.Call(
                        member,
                        typeof(string).GetMethod(nameof(string.StartsWith), new[] { typeof(string) })!,
                        constant);
                    break;

                default:
                    throw new NotSupportedException($"Unsupported search logic: {term.SearchLogic}");
            }

            // TODO: implement OR/AND logic
            combined = combined == null ? body : Expression.AndAlso(combined, body);
        }

        return combined == null ? (e) => true : Expression.Lambda<Func<T, bool>>(combined, parameter);
    }
}
