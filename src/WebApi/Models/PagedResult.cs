namespace JonathanPotts.RecipeBook.WebApi.Models;

public record PagedResult<T>(int Total, IEnumerable<T> Items);
