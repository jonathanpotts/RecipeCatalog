using System.Net.Http.Json;
using System.Security.Claims;
using JonathanPotts.RecipeCatalog.Application.Contracts.Models;
using JonathanPotts.RecipeCatalog.Application.Contracts.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace JonathanPotts.RecipeCatalog.BlazorApp.Client.Services;

public class RecipeService(HttpClient client) : IRecipeService
{
    public async Task<PagedResult<RecipeWithCuisineDto>> GetListAsync(
        int? skip = 0,
        int? take = 20,
        int[]? cuisineIds = null,
        bool? withDetails = null,
        CancellationToken cancellationToken = default)
    {
        var uri = QueryHelpers.AddQueryString(
            "/api/v1/recipes",
            new Dictionary<string, StringValues>
            {
                { "skip", skip?.ToString() },
                { "take", take?.ToString() },
                { "cuisineIds", cuisineIds?.Select(x => x.ToString()).ToArray() },
                { "withDetails", withDetails?.ToString() },
            });

        return await client.GetFromJsonAsync<PagedResult<RecipeWithCuisineDto>>(
            uri,
            cancellationToken)
            ?? throw new Exception();
    }

    public async Task<RecipeWithCuisineDto?> GetAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        return await client.GetFromJsonAsync<RecipeWithCuisineDto>(
            $"/api/v1/recipes/{id}",
            cancellationToken);
    }

    public Task<string> GetCoverImageAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<RecipeWithCuisineDto> CreateAsync(
        CreateUpdateRecipeDto dto,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default)
    {
        var response = await client.PostAsJsonAsync(
            "/api/v1/recipes",
            dto,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<RecipeWithCuisineDto>(cancellationToken)
            ?? throw new Exception();
    }

    public async Task<RecipeWithCuisineDto> UpdateAsync
        (long id,
        CreateUpdateRecipeDto dto,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default)
    {
        var response = await client.PutAsJsonAsync(
            $"/api/v1/recipes/{id}",
            dto,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<RecipeWithCuisineDto>(cancellationToken)
            ?? throw new Exception();
    }

    public async Task DeleteAsync(
        long id,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default)
    {
        var response = await client.DeleteAsync(
            $"/api/v1/recipes/{id}",
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }
}
