using IdGen;
using JonathanPotts.RecipeCatalog.Application.Contracts.Models;
using JonathanPotts.RecipeCatalog.WebApi.Apis;
using JonathanPotts.RecipeCatalog.WebApi.Data;
using JonathanPotts.RecipeCatalog.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace JonathanPotts.RecipeCatalog.BlazorApp.Services;

public class ServerRecipesService(
    ApplicationDbContext context,
    IdGenerator idGenerator,
    IAuthorizationService authorizationService,
    UserManager<ApplicationUser> userManager)
    : IRecipesService
{
    private readonly RecipesApi.Services _services = new(context, idGenerator, authorizationService, userManager);

    public async Task<PagedResult<RecipeWithCuisineDto>> GetListAsync(
        int? top = null,
        long? last = null,
        int[]? cuisineIds = null,
        bool? withDetails = null)
    {
        var result = await RecipesApi.GetListAsync(_services, top, last, cuisineIds, withDetails);

        if (result.Result is Ok<PagedResult<RecipeWithCuisineDto>> okResult && okResult.Value != null)
        {
            return okResult.Value;
        }

        throw new Exception("Unable to retrieve recipes.");
    }
}
