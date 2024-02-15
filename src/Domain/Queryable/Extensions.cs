namespace JonathanPotts.RecipeCatalog.Domain.Queryable;

public static class Extensions
{
    public static IQueryable<T> ApplyOrdering<T>(
        this IQueryable<T> queryable,
        List<Ordering<T>>? orderBy)
        where T : class
    {
        if (orderBy == null || orderBy.Count == 0)
        {
            return queryable;
        }

        var orderedQueryable = orderBy[0].Descending
            ? queryable.OrderByDescending(orderBy[0].KeySelector)
            : queryable.OrderBy(orderBy[0].KeySelector);

        foreach (var ordering in orderBy.Skip(1) ?? [])
        {
            orderedQueryable = ordering.Descending
                ? orderedQueryable.ThenByDescending(ordering.KeySelector)
                : orderedQueryable.ThenBy(ordering.KeySelector);
        }

        return orderedQueryable;
    }
}
