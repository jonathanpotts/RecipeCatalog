using FluentValidation;
using JonathanPotts.RecipeCatalog.Application.Contracts.Models;
using JonathanPotts.RecipeCatalog.Application.Contracts.Services;
using JonathanPotts.RecipeCatalog.Application.Mapping;
using JonathanPotts.RecipeCatalog.Domain.Entities;
using JonathanPotts.RecipeCatalog.Domain.Repositories;

namespace JonathanPotts.RecipeCatalog.Application.Services;

public class RecipeService(IRepository<Recipe> repository) : IRecipeService
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
                .LessThanOrEqualTo(IRecipeService.MaxItemsPerPage);
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
            recipes.Select(
                x => x.ToRecipeWithCuisineDto(withDetails.GetValueOrDefault(false))));
    }

    public async Task<RecipeWithCuisineDto?> GetAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var recipe = await repository.FirstOrDefaultAsync(
            x => x.Id == id,
            [x => x.Cuisine],
            true,
            cancellationToken);

        return recipe?.ToRecipeWithCuisineDto();
    }
}
