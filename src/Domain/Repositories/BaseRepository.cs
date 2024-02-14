using System.Linq.Expressions;
using JonathanPotts.RecipeCatalog.Domain.EntityFrameworkCore;
using JonathanPotts.RecipeCatalog.Domain.Queryable;
using Microsoft.EntityFrameworkCore;

namespace JonathanPotts.RecipeCatalog.Domain.Repositories;

public abstract class BaseRepository<T>(RecipeCatalogDbContext context)
    : IRepository<T> where T : class
{
    protected RecipeCatalogDbContext Context { get; } = context;

    protected DbSet<T> DbSet { get; } = context.Set<T>();

    public async Task<List<T>> GetListAsync(bool noTracking = false, CancellationToken cancellationToken = default)
    {
        return await DbSet.ApplyNoTracking(noTracking).ToListAsync(cancellationToken);
    }

    public async Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate, bool noTracking = false, CancellationToken cancellationToken = default)
    {
        return await DbSet.ApplyNoTracking(noTracking).Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<List<T>> GetPagedListAsync(int skip, int take, List<Ordering<T>>? orderBy = null, bool noTracking = false, CancellationToken cancellationToken = default)
    {
        return await DbSet.ApplyNoTracking(noTracking).ApplyOrdering(orderBy).Skip(skip).Take(take).ToListAsync(cancellationToken);
    }

    public async Task<List<T>> GetPagedListAsync(Expression<Func<T, bool>> predicate, int skip, int take, List<Ordering<T>>? orderBy = null, bool noTracking = false, CancellationToken cancellationToken = default)
    {
        return await DbSet.ApplyNoTracking(noTracking).Where(predicate).ApplyOrdering(orderBy).Skip(skip).Take(take).ToListAsync(cancellationToken);
    }

    public async Task<T?> FindAsync(object?[]? keyValues, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync(keyValues, cancellationToken);
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        var update = DbSet.Update(entity);
        await Context.SaveChangesAsync(cancellationToken);

        return update.Entity;
    }

    public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        DbSet.Remove(entity);
        await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        DbSet.RemoveRange(DbSet.Where(predicate));
        await Context.SaveChangesAsync(cancellationToken);
    }
}
