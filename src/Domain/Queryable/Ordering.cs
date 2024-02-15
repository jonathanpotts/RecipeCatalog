using System.Linq.Expressions;

namespace JonathanPotts.RecipeCatalog.Domain.Queryable;

public class Ordering<T>
{
    public Expression<Func<T, object?>> KeySelector { get; init; }

    public bool Descending { get; init; }

    public Ordering(Expression<Func<T, object?>> keySelector, bool descending = false)
    {
        KeySelector = keySelector;
        Descending = descending;
    }

    public Ordering(string propertyName, bool descending = false)
    {
        var paramExpr = Expression.Parameter(typeof(T));
        var propExpr = Expression.Property(paramExpr, propertyName);
        KeySelector = Expression.Lambda<Func<T, object?>>(propExpr, paramExpr);
        Descending = descending;
    }
}
