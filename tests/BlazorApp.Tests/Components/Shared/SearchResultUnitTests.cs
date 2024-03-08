using Bunit;
using JonathanPotts.RecipeCatalog.Application.Contracts.Models;
using JonathanPotts.RecipeCatalog.BlazorApp.Components.Shared;
using JonathanPotts.RecipeCatalog.Domain.Shared.ValueObjects;

namespace JonathanPotts.RecipeCatalog.BlazorApp.Tests.Components.Shared;

public sealed class SearchResultUnitTests
{
    [Fact]
    public void SearchResultRendersCorrectly()
    {
        // Arrange
        RecipeWithCuisineDto recipe = new()
        {
            Id = 6461870173061120,
            OwnerId = "d7df5331-1c53-491f-8b71-91989846874f",
            Name = "Test Recipe 1",
            CoverImage = new ImageData
            {
                Url = "6461870173061120.webp",
                AltText = "A photo of test recipe 1"
            },
            Cuisine = new CuisineDto
            {
                Id = 1,
                Name = "Test"
            },
            Description = "This is a test.",
            Created = new DateTime(638412046299055561, DateTimeKind.Utc),
            Ingredients =
            [
                "1 tsp of test ingredient 1",
                "1 cup of test ingredient 2"
            ],
            Instructions = new MarkdownData
            {
                Markdown = "This is a test.",
                Html = "<p>This is a test.</p>\n"
            }
        };

        var ctx = new TestContext();

        // Act
        var cut = ctx.RenderComponent<SearchResult>(builder =>
        {
            builder.Add(x => x.Recipe, recipe);
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
