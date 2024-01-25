namespace JonathanPotts.RecipeCatalog.WebApi.Models;

public record PagedResult<T>(int Total, IEnumerable<T> Items);
