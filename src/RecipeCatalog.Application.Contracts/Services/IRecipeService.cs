using System.Security.Claims;
using RecipeCatalog.Application.Contracts.Models;

namespace RecipeCatalog.Application.Contracts.Services;

public interface IRecipeService
{
    public const int MaxItemsPerPage = 50;

    public Task<PagedResult<RecipeWithCuisineDto>> GetListAsync(
        int? skip = null,
        int? take = null,
        int[]? cuisineIds = null,
        bool? withDetails = null,
        CancellationToken cancellationToken = default);

    public Task<RecipeWithCuisineDto?> GetAsync(
        long id,
        CancellationToken cancellationToken = default);

    public Task<string?> GetCoverImageAsync(
        long id,
        CancellationToken cancellationToken = default);

    public Task<bool> UpdateCoverImageAsync(
        long id,
        Stream imageData,
        string? description,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default);

    public Task<bool> DeleteCoverImageAsync(
        long id,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default);

    public Task<RecipeWithCuisineDto> CreateAsync(
        CreateUpdateRecipeDto dto,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default);

    public Task<RecipeWithCuisineDto?> UpdateAsync(
        long id,
        CreateUpdateRecipeDto dto,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default);

    public Task<bool> DeleteAsync(
        long id,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default);

    public Task<PagedResult<RecipeWithCuisineDto>> SearchAsync(
        string query,
        int? skip = null,
        int? take = null,
        CancellationToken cancellationToken = default);
}
