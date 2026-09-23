using System.Linq.Expressions;

namespace VictoryCenter.BLL.Services.Search.Helpers;
public static class ExpressionExtensions
{
    public static Expression<Func<T, bool>> Or<T>(
        this Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right)
    {
        var parameter = Expression.Parameter(typeof(T));

        var leftBody = new ReplaceParameterVisitor(left.Parameters[0], parameter).Visit(left.Body);
        var rightBody = new ReplaceParameterVisitor(right.Parameters[0], parameter).Visit(right.Body);

        var orElse = Expression.OrElse(leftBody!, rightBody!);

        return Expression.Lambda<Func<T, bool>>(orElse, parameter);
    }
}
