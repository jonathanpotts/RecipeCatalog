using System.Linq.Expressions;
using JonathanPotts.RecipeCatalog.Domain.Queryable;

namespace JonathanPotts.RecipeCatalog.Domain.Repositories;

public interface IRepository<T> where T : class
{
    public Task<List<T>> GetListAsync(bool noTracking = false, CancellationToken cancellationToken = default);

    public Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate, bool noTracking = false, CancellationToken cancellationToken = default);

    public Task<List<T>> GetPagedListAsync(int skip, int take, List<Ordering<T>>? orderBy = null, bool noTracking = false, CancellationToken cancellationToken = default);

    public Task<List<T>> GetPagedListAsync(Expression<Func<T, bool>> predicate, int skip, int take, List<Ordering<T>>? orderBy = null, bool noTracking = false, CancellationToken cancellationToken = default);

    public Task<T?> FindAsync(object?[]? keyValues, CancellationToken cancellationToken = default);

    public Task AddAsync(T entity, CancellationToken cancellationToken = default);

    public Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);

    public Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

    public Task DeleteAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
}
