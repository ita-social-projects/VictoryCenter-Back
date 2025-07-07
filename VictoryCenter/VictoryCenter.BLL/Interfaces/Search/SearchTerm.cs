using System.Linq.Expressions;

namespace VictoryCenter.BLL.Interfaces.Search;

// Search term can represent either a separate column or another token within same column term
public class SearchTerm<TEntity>
{
    public Expression<Func<TEntity, string>> TermSelector { get; set; } = null!;
    public string? TermValue { get; set; }
    public SearchLogic SearchLogic { get; set; }
}
