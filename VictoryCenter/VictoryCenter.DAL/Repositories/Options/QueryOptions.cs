using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace VictoryCenter.DAL.Repositories.Options;

public class QueryOptions<T>
{
    public Expression<Func<T, bool>>? Filter { get; set; }
    public Func<IQueryable<T>, IIncludableQueryable<T, object>>? Include { get; set; }
    public int Offset { get; set; }
    public int Limit { get; set; }
    public Expression<Func<T, object>>? OrderByASC { get; set; }
    public Expression<Func<T, object>>? OrderByDESC { get; set; }
    public Expression<Func<T, T>>? Selector { get; set; }
}
