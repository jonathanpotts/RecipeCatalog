namespace JonathanPotts.RecipeCatalog.Domain.Queryable;

public static class Extensions
{
    public static IQueryable<T> ApplyOrdering<T>(this IQueryable<T> queryable, List<Ordering<T>>? orderBy) where T : class
    {
        foreach (var ordering in orderBy ?? Enumerable.Empty<Ordering<T>>())
        {
            if (queryable is IOrderedQueryable<T> orderedQueryable)
            {
                queryable = ordering.Descending
                    ? orderedQueryable.ThenByDescending(ordering.KeySelector)
                    : orderedQueryable.ThenBy(ordering.KeySelector);
            }
            else
            {
                queryable = ordering.Descending
                    ? queryable.OrderByDescending(ordering.KeySelector)
                    : queryable.OrderBy(ordering.KeySelector);
            }
        }

        return queryable;
    }
}
