using JonathanPotts.RecipeCatalog.Application.Contracts.Models;
using JonathanPotts.RecipeCatalog.Application.Contracts.Services;
using JonathanPotts.RecipeCatalog.Domain.Entities;
using JonathanPotts.RecipeCatalog.Domain.Repositories;
using JonathanPotts.RecipeCatalog.Domain.Shared.ValueObjects;

namespace JonathanPotts.RecipeCatalog.Application.Services;

public class RecipesService(IRepository<Recipe> repository) : IRecipesService
{
    public async Task<PagedResult<RecipeWithCuisineDto>> GetPagedResultAsync(
        int skip = 0,
        int take = 20,
        int[]? cuisineIds = null,
        bool? withDetails = null,
        CancellationToken cancellationToken = default)
    {
        var recipes = (cuisineIds?.Length > 0)
            ? await repository.GetPagedListAsync(
                x => cuisineIds.Contains(x.CuisineId),
                skip,
                take,
                noTracking: true,
                cancellationToken: cancellationToken)
            : await repository.GetPagedListAsync(
                skip,
                take,
                noTracking: true,
                cancellationToken: cancellationToken);

        return new PagedResult<RecipeWithCuisineDto>(0,
            recipes.Select(x => new RecipeWithCuisineDto
            {
                Id = x.Id,
                OwnerId = x.OwnerId,
                Name = x.Name,
                CoverImage = x.CoverImage == null
                ? null
                : new ImageData
                {
                    Url = $"/api/v1/recipes/{x.Id}/coverImage",
                    AltText = x.CoverImage.AltText
                },
                Cuisine = x.Cuisine == null
                ? null
                : new CuisineDto
                {
                    Id = x.Cuisine.Id,
                    Name = x.Cuisine.Name
                },
                Description = withDetails.GetValueOrDefault(false) ? x.Description : null,
                Created = x.Created,
                Modified = x.Modified,
                Ingredients = withDetails.GetValueOrDefault(false) ? x.Ingredients : null,
                Instructions = withDetails.GetValueOrDefault(false) ? x.Instructions : null
            }));
    }
}
