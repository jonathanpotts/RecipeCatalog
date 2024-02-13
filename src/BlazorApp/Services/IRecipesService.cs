using JonathanPotts.RecipeCatalog.Shared.Models;

namespace JonathanPotts.RecipeCatalog.BlazorApp.Services;

public interface IRecipesService
{
    public Task<PagedResult<RecipeWithCuisineDto>> GetListAsync(
        int? top = null,
        long? last = null,
        int[]? cuisineIds = null,
        bool? withDetails = null);
}
