using FluentValidation;
using JonathanPotts.RecipeCatalog.Application.Contracts.Models;
using JonathanPotts.RecipeCatalog.Application.Contracts.Services;
using JonathanPotts.RecipeCatalog.Domain.Entities;
using JonathanPotts.RecipeCatalog.Domain.Repositories;
using JonathanPotts.RecipeCatalog.Domain.Shared.ValueObjects;

namespace JonathanPotts.RecipeCatalog.Application.Services;

public class RecipesService(IRepository<Recipe> repository) : IRecipesService
{
    private const int DefaultItemsPerPage = 20;

    public async Task<PagedResult<RecipeWithCuisineDto>> GetPagedResultAsync(
        int? skip = null,
        int? take = null,
        int[]? cuisineIds = null,
        bool? withDetails = null,
        CancellationToken cancellationToken = default)
    {
        if (skip != null)
        {
            InlineValidator<int> validator = [];
            validator.RuleFor(x => x).GreaterThanOrEqualTo(0);
            validator.ValidateAndThrow(skip.Value);
        }

        if (take != null)
        {
            InlineValidator<int> validator = [];
            validator.RuleFor(x => x)
                .GreaterThan(0)
                .LessThanOrEqualTo(IRecipesService.MaxItemsPerPage);
            validator.ValidateAndThrow(take.Value);
        }

        skip ??= 0;
        take ??= DefaultItemsPerPage;

        var count = (cuisineIds?.Length > 0)
            ? await repository.CountAsync(
                x => cuisineIds.Contains(x.CuisineId),
                cancellationToken)
            : await repository.CountAsync(cancellationToken);

        var recipes = (cuisineIds?.Length > 0)
            ? await repository.GetPagedListAsync(
                x => cuisineIds.Contains(x.CuisineId),
                skip.Value,
                take.Value,
                [new(x => x.Id, true)],
                [x => x.Cuisine],
                true,
                cancellationToken)
            : await repository.GetPagedListAsync(
                skip.Value,
                take.Value,
                [new(x => x.Id, true)],
                [x => x.Cuisine],
                true,
                cancellationToken);

        return new PagedResult<RecipeWithCuisineDto>(
            count,
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
                Ingredients = withDetails.GetValueOrDefault(false)
                    ? x.Ingredients
                    : null,
                Instructions = withDetails.GetValueOrDefault(false)
                    ? x.Instructions
                    : null
            }));
    }
}
