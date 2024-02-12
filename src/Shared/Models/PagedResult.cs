namespace JonathanPotts.RecipeCatalog.Shared.Models;

public record PagedResult<T>(int Total, IEnumerable<T> Items);
