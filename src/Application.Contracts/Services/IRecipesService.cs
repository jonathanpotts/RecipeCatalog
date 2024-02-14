using JonathanPotts.RecipeCatalog.Application.Contracts.Models;

namespace JonathanPotts.RecipeCatalog.Application.Contracts.Services;

public interface IRecipesService
{
    public Task<PagedResult<RecipeWithCuisineDto>> GetListAsync(
        int? top = null,
        long? last = null,
        int[]? cuisineIds = null,
        bool? withDetails = null);
}
