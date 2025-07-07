using System.Linq.Expressions;

namespace VictoryCenter.BLL.Interfaces.Search;

public interface ISearchService<T>
    where T : class
{
    Expression<Func<T, bool>> CreateSearchExpression(params SearchTerm<T>[] searchTerms);
}
