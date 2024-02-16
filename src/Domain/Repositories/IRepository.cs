using System.Linq.Expressions;
using JonathanPotts.RecipeCatalog.Domain.Queryable;

namespace JonathanPotts.RecipeCatalog.Domain.Repositories;

public interface IRepository<T> where T : class
{
    public Task<List<T>> GetListAsync(
        List<Expression<Func<T, object?>>>? include = null,
        bool noTracking = false,
        CancellationToken cancellationToken = default);

    public Task<List<T>> GetListAsync(
        Expression<Func<T, bool>> predicate,
        List<Expression<Func<T, object?>>>? include = null,
        bool noTracking = false,
        CancellationToken cancellationToken = default);

    public Task<List<T>> GetPagedListAsync(
        int skip,
        int take,
        List<Ordering<T>>? orderBy = null,
        List<Expression<Func<T, object?>>>? include = null,
        bool noTracking = false,
        CancellationToken cancellationToken = default);

    public Task<List<T>> GetPagedListAsync(
        Expression<Func<T, bool>> predicate,
        int skip,
        int take,
        List<Ordering<T>>? orderBy = null,
        List<Expression<Func<T, object?>>>? include = null,
        bool noTracking = false,
        CancellationToken cancellationToken = default);

    public Task<int> CountAsync(CancellationToken cancellationToken = default);

    public Task<int> CountAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    public Task<long> LongCountAsync(CancellationToken cancellationToken = default);

    public Task<long> LongCountAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    public Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>>? predicate = null,
        List<Expression<Func<T, object?>>>? include = null,
        bool noTracking = false,
        CancellationToken cancellationToken = default);

    public Task AddAsync(
        T entity,
        bool saveChanges = false,
        CancellationToken cancellationToken = default);

    public Task AddRangeAsync(
        IEnumerable<T> entities,
        bool saveChanges = false,
        CancellationToken cancellationToken = default);

    public Task UpdateAsync(
        T entity,
        bool saveChanges = false,
        CancellationToken cancellationToken = default);

    public Task UpdateRangeAsync(
        IEnumerable<T> entities,
        bool saveChanges = false,
        CancellationToken cancellationToken = default);

    public Task DeleteAsync(
        T entity,
        bool saveChanges = false,
        CancellationToken cancellationToken = default);

    public Task DeleteAsync(
        Expression<Func<T, bool>> predicate,
        bool saveChanges = false,
        CancellationToken cancellationToken = default);

    public Task DeleteRangeAsync(
        IEnumerable<T> entities,
        bool saveChanges = false,
        CancellationToken cancellationToken = default);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
