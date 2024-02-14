using System.Linq.Expressions;

namespace JonathanPotts.RecipeCatalog.Domain.Services;

public interface IRepository<T> where T : class
{
    public Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);

    public Task<T?> FindAsync(object?[]? keyValues, CancellationToken cancellationToken = default);

    public Task AddAsync(T entity, CancellationToken cancellationToken = default);

    public Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);

    public Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

    public Task DeleteAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
}
