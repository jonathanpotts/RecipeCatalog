using System.Net.Http.Json;
using System.Security.Claims;
using RecipeCatalog.Application.Contracts.Models;
using RecipeCatalog.Application.Contracts.Services;

namespace RecipeCatalog.BlazorApp.Client.Services;

public class CuisineService(HttpClient client) : ICuisineService
{
    public async Task<IEnumerable<CuisineDto>> GetListAsync(
        CancellationToken cancellationToken = default)
    {
        return await client.GetFromJsonAsync<CuisineDto[]>(
            "/api/v1/cuisines",
            cancellationToken)
            ?? throw new Exception();
    }

    public async Task<CuisineDto?> GetAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await client.GetFromJsonAsync<CuisineDto>(
            $"/api/v1/cuisines/{id}",
            cancellationToken);
    }

    public async Task<CuisineDto> CreateAsync(
        CreateUpdateCuisineDto dto,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default)
    {
        var response = await client.PostAsJsonAsync(
            "/api/v1/cuisines",
            dto,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<CuisineDto>(cancellationToken)
            ?? throw new Exception();
    }

    public async Task<CuisineDto?> UpdateAsync(
        int id,
        CreateUpdateCuisineDto dto,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default)
    {
        var response = await client.PutAsJsonAsync(
            $"/api/v1/cuisines/{id}",
            dto,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<CuisineDto>(cancellationToken);
    }

    public async Task<bool> DeleteAsync(
        int id,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default)
    {
        var response = await client.DeleteAsync(
            $"/api/v1/cuisines/{id}",
            cancellationToken);

        return response.IsSuccessStatusCode;
    }
}
