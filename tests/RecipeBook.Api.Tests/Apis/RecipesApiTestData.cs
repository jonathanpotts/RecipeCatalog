using RecipeBook.Api.Models;

namespace RecipeBook.Api.Tests.Apis;

public static class RecipesApiTestData
{
    public static readonly Recipe[] Recipes =
    [
        new Recipe
        {
            Id = 6461870173061120,
            Name = "Test Recipe 1",
            Cuisine = "Test",
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
        },
        new Recipe
        {
            Id = 6462160192405504,
            Name = "Test Recipe 2",
            Cuisine = "Test",
            Description = "This is a test.",
            Created = new DateTime(638412046990521543, DateTimeKind.Utc),
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
        },
        new Recipe
        {
            Id = 6462258523668480,
            Name = "Test Recipe 3",
            Cuisine = "Test",
            Description = "This is a test.",
            Created = new DateTime(638412047224957774, DateTimeKind.Utc),
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
        },
        new Recipe
        {
            Id = 6462318867120128,
            Name = "Test Recipe 4",
            Cuisine = "Test",
            Description = "This is a test.",
            Created = new DateTime(638412047368832961, DateTimeKind.Utc),
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
        },
        new Recipe
        {
            Id = 6462416804118528,
            Name = "Test Recipe 5",
            Cuisine = "Test",
            Description = "This is a test.",
            Created = new DateTime(638412047602332665, DateTimeKind.Utc),
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
        }
    ];
}
