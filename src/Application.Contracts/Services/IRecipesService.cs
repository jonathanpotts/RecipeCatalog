using JonathanPotts.RecipeCatalog.Application.Contracts.Models;

namespace JonathanPotts.RecipeCatalog.Application.Contracts.Services;

public interface IRecipesService
{
    public Task<PagedResult<RecipeWithCuisineDto>> GetPagedResultAsync(
        int skip = 0,
        int take = 20,
        int[]? cuisineIds = null,
        bool? withDetails = null,
        CancellationToken cancellationToken = default);
}
