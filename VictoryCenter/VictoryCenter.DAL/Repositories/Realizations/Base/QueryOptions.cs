using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace VictoryCenter.DAL.Repositories.Realizations.Base;

public class QueryOptions<T>
{
    public Expression<Func<T, bool>>? FilterPredicate { get; set; }
    public Func<IQueryable<T>, IIncludableQueryable<T, object>>? Include { get; set; }
}
