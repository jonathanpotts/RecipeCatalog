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

    public async Task<List<T>> GetListAsync(
        List<Expression<Func<T, object?>>>? include = null,
        bool noTracking = false,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .ApplyNoTracking(noTracking)
            .ApplyEagerLoading(include)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<T>> GetListAsync(
        Expression<Func<T, bool>> predicate,
        List<Expression<Func<T, object?>>>? include = null,
        bool noTracking = false,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .ApplyNoTracking(noTracking)
            .ApplyEagerLoading(include)
            .Where(predicate)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<T>> GetPagedListAsync(
        int skip,
        int take,
        List<Ordering<T>>? orderBy = null,
        List<Expression<Func<T, object?>>>? include = null,
        bool noTracking = false,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .ApplyNoTracking(noTracking)
            .ApplyEagerLoading(include)
            .ApplyOrdering(orderBy)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<T>> GetPagedListAsync(
        Expression<Func<T, bool>> predicate,
        int skip,
        int take,
        List<Ordering<T>>? orderBy = null,
        List<Expression<Func<T, object?>>>? include = null,
        bool noTracking = false,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .ApplyNoTracking(noTracking)
            .ApplyEagerLoading(include)
            .Where(predicate)
            .ApplyOrdering(orderBy)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.CountAsync(predicate, cancellationToken);
    }

    public async Task<long> LongCountAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.LongCountAsync(cancellationToken);
    }

    public async Task<long> LongCountAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.LongCountAsync(predicate, cancellationToken);
    }

    public async Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>>? predicate = null,
        List<Expression<Func<T, object?>>>? include = null,
        bool noTracking = false,
        CancellationToken cancellationToken = default)
    {
        var queryable = DbSet.ApplyNoTracking(noTracking).ApplyEagerLoading(include);

        return predicate == null
            ? await queryable.FirstOrDefaultAsync(cancellationToken)
            : await queryable.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task AddAsync(
        T entity,
        bool saveChanges = false,
        CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);

        if (saveChanges)
        {
            await SaveChangesAsync(cancellationToken);
        }
    }

    public async Task AddRangeAsync(
        IEnumerable<T> entities,
        bool saveChanges = false,
        CancellationToken cancellationToken = default)
    {
        await DbSet.AddRangeAsync(entities, cancellationToken);

        if (saveChanges)
        {
            await SaveChangesAsync(cancellationToken);
        }
    }

    public async Task UpdateAsync(
        T entity,
        bool saveChanges = false,
        CancellationToken cancellationToken = default)
    {
        DbSet.Update(entity);

        if (saveChanges)
        {
            await SaveChangesAsync(cancellationToken);
        }
    }

    public async Task UpdateRangeAsync(
        IEnumerable<T> entities,
        bool saveChanges = false,
        CancellationToken cancellationToken = default)
    {
        DbSet.UpdateRange(entities);

        if (saveChanges)
        {
            await SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DeleteAsync(
        T entity,
        bool saveChanges = false,
        CancellationToken cancellationToken = default)
    {
        DbSet.Remove(entity);

        if (saveChanges)
        {
            await SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DeleteAsync(
        Expression<Func<T, bool>> predicate,
        bool saveChanges = false,
        CancellationToken cancellationToken = default)
    {
        DbSet.RemoveRange(DbSet.Where(predicate));

        if (saveChanges)
        {
            await SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DeleteRangeAsync(
        IEnumerable<T> entities,
        bool saveChanges = false,
        CancellationToken cancellationToken = default)
    {
        DbSet.RemoveRange(entities);

        if (saveChanges)
        {
            await SaveChangesAsync(cancellationToken);
        }
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await Context.SaveChangesAsync(cancellationToken);
    }
}
