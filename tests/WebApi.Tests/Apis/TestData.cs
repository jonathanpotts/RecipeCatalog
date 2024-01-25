using JonathanPotts.RecipeBook.WebApi.Models;

namespace JonathanPotts.RecipeBook.WebApi.Tests.Apis;

public static class TestData
{
    public static readonly Cuisine[] Cuisines =
    [
        new Cuisine
        {
            Id = 1,
            Name = "Test"
        },
        new Cuisine
        {
            Id = 2,
            Name = "New"
        },
        new Cuisine
        {
            Id = 3,
            Name = "Updated"
        },
        new Cuisine
        {
            Id = 4,
            Name = "Test 2"
        }
    ];

    public static readonly Recipe[] Recipes =
    [
        new Recipe
        {
            Id = 6461870173061120,
            Name = "Test Recipe 1",
            CuisineId = 1,
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
            CuisineId = 1,
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
            CuisineId = 1,
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
            CuisineId = 4,
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
            CuisineId = 1,
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
