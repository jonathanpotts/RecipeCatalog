using JonathanPotts.RecipeCatalog.Application.Contracts.Models;
using JonathanPotts.RecipeCatalog.Domain.Entities;
using JonathanPotts.RecipeCatalog.Domain.Shared.ValueObjects;

namespace JonathanPotts.RecipeCatalog.Application.Mapping;

public static class RecipeExtensions
{
    public static RecipeWithCuisineDto ToRecipeWithCuisineDto(this Recipe recipe, bool withDetails = true)
    {
        return new RecipeWithCuisineDto
        {
            Id = recipe.Id,
            OwnerId = recipe.OwnerId,
            Name = recipe.Name,
            CoverImage = recipe.CoverImage == null
                ? null
                : new ImageData
                {
                    Url = $"/api/v1/recipes/{recipe.Id}/coverImage",
                    AltText = recipe.CoverImage.AltText,
                },
            Cuisine = recipe.Cuisine == null
                ? null
                : new CuisineDto
                {
                    Id = recipe.Cuisine.Id,
                    Name = recipe.Cuisine.Name,
                },
            Description = recipe.Description,
            Created = recipe.Created,
            Modified = recipe.Modified,
            Ingredients = withDetails ? recipe.Ingredients : null,
            Instructions = withDetails ? recipe.Instructions : null,
        };
    }

    public static Recipe ToRecipe(this CreateUpdateRecipeDto dto)
    {
        return new Recipe
        {
            Name = dto.Name,
            CuisineId = dto.CuisineId,
            Description = dto.Description,
            Ingredients = dto.Ingredients,
            Instructions = new MarkdownData
            {
                Markdown = dto.Instructions,
            },
        };
    }
}
