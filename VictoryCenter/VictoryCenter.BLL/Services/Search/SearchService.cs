using System.Linq.Expressions;
using VictoryCenter.BLL.Interfaces.Search;

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
        var parameter = Expression.Parameter(typeof(T), "e");
        Expression? combined = null;

        foreach (var term in searchTerms)
        {
            if (string.IsNullOrEmpty(term.TermValue))
            {
                continue;
            }

            var member = new ReplaceParameterVisitor(term.TermSelector.Parameters[0], parameter)
                .Visit(term.TermSelector.Body)!;
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

            combined = combined == null ? body : Expression.AndAlso(combined, body);
        }

        return combined == null ? (e) => true : Expression.Lambda<Func<T, bool>>(combined, parameter);
    }
}
