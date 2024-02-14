using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace JonathanPotts.RecipeCatalog.Domain.Services;

public abstract class BaseRepository<T>(RecipeCatalogDbContext context)
    : IRepository<T> where T : class
{
    protected RecipeCatalogDbContext Context { get; } = context;

    protected DbSet<T> DbSet { get; } = context.Set<T>();

    public async Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.ToListAsync(cancellationToken);
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
