using Bunit;
using RecipeCatalog.Application.Mapping;
using RecipeCatalog.BlazorApp.Components.Shared;
using RecipeCatalog.Tests.Shared;

namespace RecipeCatalog.BlazorApp.Tests.Components.Shared;

public sealed class SearchResultUnitTests
{
    [Fact]
    public void SearchResultRendersCorrectly()
    {
        // Arrange
        var recipe = TestData.Recipes[0];
        recipe.Cuisine = TestData.Cuisines.FirstOrDefault(x => x.Id == recipe.CuisineId);
        var recipeDto = recipe.ToRecipeWithCuisineDto();

        var ctx = new TestContext();

        // Act
        var cut = ctx.RenderComponent<SearchResult>(builder =>
        {
            builder.Add(x => x.Recipe, recipeDto);
        });

        // Assert
        cut.MarkupMatches("""
            <div>
                <div class="fw-bold">
                    <a href="recipes/6461870173061120">
                        Test Recipe 1
                    </a>
                    <span class="badge text-bg-secondary">Test</span>
                </div>
                <p class="line-clamp-3">This is a test.</p>
            </div>
            """);
    }
}
