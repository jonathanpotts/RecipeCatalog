using System.Security.Claims;
using JonathanPotts.RecipeCatalog.Application.Contracts.Models;

namespace JonathanPotts.RecipeCatalog.Application.Contracts.Services;

public interface IRecipeService
{
    public const int MaxItemsPerPage = 50;

    public Task<PagedResult<RecipeWithCuisineDto>> GetListAsync(
        int? skip = 0,
        int? take = 20,
        int[]? cuisineIds = null,
        bool? withDetails = null,
        CancellationToken cancellationToken = default);

    public Task<RecipeWithCuisineDto?> GetAsync(
        long id,
        CancellationToken cancellationToken = default);

    public Task<string> GetCoverImageAsync(
        long id,
        CancellationToken cancellationToken = default);

    public Task<RecipeWithCuisineDto> CreateAsync(
        CreateUpdateRecipeDto dto,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default);

    public Task<RecipeWithCuisineDto> UpdateAsync(
        long id,
        CreateUpdateRecipeDto dto,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default);

    public Task DeleteAsync(
        long id,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default);
}
