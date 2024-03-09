using Bunit;
using JonathanPotts.RecipeCatalog.Application.Mapping;
using JonathanPotts.RecipeCatalog.BlazorApp.Client.Components.Shared;
using JonathanPotts.RecipeCatalog.Tests.Shared;

namespace JonathanPotts.RecipeCatalog.BlazorApp.Client.Tests.Components.Shared;

public sealed class RecipeCardUnitTests
{
    [Fact]
    public void RecipeCardRendersCorrectly()
    {
        // Arrange
        var recipe = TestData.Recipes[0];
        recipe.Cuisine = TestData.Cuisines.FirstOrDefault(x => x.Id == recipe.CuisineId);
        var recipeDto = recipe.ToRecipeWithCuisineDto();

        var ctx = new TestContext();

        // Act
        var cut = ctx.RenderComponent<RecipeCard>(builder =>
        {
            builder.Add(x => x.Recipe, recipeDto);
        });

        // Assert
        cut.MarkupMatches("""
            <div class="card h-100">
                <div class="card-img-top bg-secondary" style="height: 300px">
                    <img
                        src="/api/v1/recipes/6461870173061120/coverImage"
                        class="card-img-top object-fit-cover"
                        alt="A photo of test recipe 1"
                        height="300" />
                </div>
                <div class="card-body d-flex flex-column">
                    <h5 class="card-title">Test Recipe 1</h5>
                    <p class="card-text"><span class="badge text-bg-secondary">Test</span></p>
                    <p class="card-text line-clamp-3">
                        This is a test.
                    </p>
                    <div class="mt-auto">
                        <a
                            href="recipes/6461870173061120"
                            class="btn btn-primary stretched-link">
                            View more
                        </a>
                    </div>
                </div>
            </div>
            """);
    }
}
