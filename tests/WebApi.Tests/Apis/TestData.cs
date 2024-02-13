using JonathanPotts.RecipeCatalog.Shared.Models;
using JonathanPotts.RecipeCatalog.WebApi.Models;
using Microsoft.AspNetCore.Identity;

namespace JonathanPotts.RecipeCatalog.WebApi.Tests.Apis;

public static class TestData
{
    public static readonly ApplicationUser[] Users =
    [
        new ApplicationUser
        {
            Id = "d7df5331-1c53-491f-8b71-91989846874f",
            SecurityStamp = "RHML6N4NEUJRDDORVUQIEZFNWO5HEOLR",
            ConcurrencyStamp = "2774d224-bb2c-4306-a55c-4942c7de7cee",
            Email = "user@example.com",
            NormalizedEmail = "USER@EXAMPLE.COM",
            UserName = "user@example.com",
            NormalizedUserName = "USER@EXAMPLE.COM",
            EmailConfirmed = true
        },
        new ApplicationUser
        {
            Id = "73edf737-df51-4c06-ac6f-3ec6d79f1f12",
            SecurityStamp = "BNCWIPXASUB44SSFFBSFRLKXKGMYOWPZ",
            ConcurrencyStamp = "9a7683aa-51c1-402d-8c34-597b2189591c",
            Email = "admin@example.com",
            NormalizedEmail = "ADMIN@EXAMPLE.COM",
            UserName = "admin@example.com",
            NormalizedUserName = "ADMIN@EXAMPLE.COM",
            EmailConfirmed = true
        }
    ];

    public static readonly IdentityRole[] Roles =
    [
        new IdentityRole
        {
            Id = "c13cd7d8-e0ac-4176-842c-bea77a37edec",
            Name = "Administrator",
            NormalizedName = "ADMINISTRATOR"
        }
    ];

    public static readonly IdentityUserRole<string>[] UserRoles =
    [
        new IdentityUserRole<string>
        {
            RoleId = "c13cd7d8-e0ac-4176-842c-bea77a37edec",
            UserId = "73edf737-df51-4c06-ac6f-3ec6d79f1f12"
        }
    ];

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
            OwnerId = "73edf737-df51-4c06-ac6f-3ec6d79f1f12",
            Name = "Test Recipe 1",
            CoverImage = new ImageData
            {
                Url = "6461870173061120.webp",
                AltText = "A photo of test recipe 1"
            },
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
            OwnerId = "d7df5331-1c53-491f-8b71-91989846874f",
            Name = "Test Recipe 2",
            CoverImage = new ImageData
            {
                Url = "6462160192405504.webp",
                AltText = "A photo of test recipe 2"
            },
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
            OwnerId = "73edf737-df51-4c06-ac6f-3ec6d79f1f12",
            Name = "Test Recipe 3",
            CoverImage = new ImageData
            {
                Url = "6462258523668480.webp",
                AltText = "A photo of test recipe 3"
            },
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
            OwnerId = "d7df5331-1c53-491f-8b71-91989846874f",
            Name = "Test Recipe 4",
            CoverImage = new ImageData
            {
                Url = "6462318867120128.webp",
                AltText = "A photo of test recipe 4"
            },
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
            OwnerId = "73edf737-df51-4c06-ac6f-3ec6d79f1f12",
            Name = "Test Recipe 5",
            CoverImage = new ImageData
            {
                Url = "6462416804118528.webp",
                AltText = "A photo of test recipe 5"
            },
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
