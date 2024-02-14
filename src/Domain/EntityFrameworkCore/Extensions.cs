using Microsoft.EntityFrameworkCore;

namespace JonathanPotts.RecipeCatalog.Domain.EntityFrameworkCore;

public static class Extensions
{
    public static IQueryable<T> ApplyNoTracking<T>(this DbSet<T> dbSet, bool noTracking) where T : class
    {
        return noTracking
            ? dbSet.AsNoTracking()
            : dbSet;
    }
}
