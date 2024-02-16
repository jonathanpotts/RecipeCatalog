using JonathanPotts.RecipeCatalog.Application.Contracts.Models;

namespace JonathanPotts.RecipeCatalog.Application.Contracts.Services;

public interface IRecipeService
{
    public const int MaxItemsPerPage = 50;

    public Task<PagedResult<RecipeWithCuisineDto>> GetPagedResultAsync(
        int? skip = 0,
        int? take = 20,
        int[]? cuisineIds = null,
        bool? withDetails = null,
        CancellationToken cancellationToken = default);

    public Task<RecipeWithCuisineDto?> GetAsync(
        long id,
        CancellationToken cancellationToken = default);
}
