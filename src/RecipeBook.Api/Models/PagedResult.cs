namespace RecipeBook.Api.Models;

public record PagedResult<T>(int Total, IEnumerable<T> Items);
