using System.Linq.Expressions;

namespace JonathanPotts.RecipeCatalog.Domain.Queryable;

public record Ordering<T>(Expression<Func<T, object?>> KeySelector, bool Descending);
