namespace JonathanPotts.RecipeCatalog.Application.Contracts.Models;

public record PagedResult<T>(int Total, IEnumerable<T> Items);
