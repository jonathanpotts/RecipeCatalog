using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace JonathanPotts.RecipeCatalog.Domain.EntityFrameworkCore;

public static class Extensions
{
    public static IQueryable<T> ApplyNoTracking<T>(
        this DbSet<T> dbSet,
        bool noTracking)
        where T : class
    {
        return noTracking
            ? dbSet.AsNoTracking()
            : dbSet;
    }

    public static IQueryable<T> ApplyEagerLoading<T>(
        this IQueryable<T> queryable,
        List<Expression<Func<T, object?>>>? navigationPropertyPaths)
        where T : class
    {
        foreach (var navigationPropertyPath in navigationPropertyPaths
            ?? Enumerable.Empty<Expression<Func<T, object?>>>())
        {
            queryable = queryable.Include(navigationPropertyPath);
        }

        return queryable;
    }
}
