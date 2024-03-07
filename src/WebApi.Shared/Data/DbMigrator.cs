using System.Text.Json;
using IdGen;
using JonathanPotts.RecipeCatalog.Domain;
using JonathanPotts.RecipeCatalog.Domain.Entities;
using Markdig;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace JonathanPotts.RecipeCatalog.WebApi.Shared.Data;

public class DbMigrator(
    IConfiguration configuration,
    RecipeCatalogDbContext context,
    IdGenerator idGenerator,
    UserManager<User> userManager,
    RoleManager<IdentityRole> roleManager)
{
    private static readonly string s_dataDirectory = Path.Combine(AppContext.BaseDirectory, "Data");
    private static readonly string s_jsonFile = Path.Combine(s_dataDirectory, "Cuisines.json");
    private static readonly string s_dataImagesDirectory = Path.Combine(s_dataDirectory, "Images");
    private static readonly string s_imagesDirectory = Path.Combine(AppContext.BaseDirectory, "Images");

    private static readonly MarkdownPipeline s_pipeline = new MarkdownPipelineBuilder()
        .DisableHtml()
        .UseReferralLinks(["ugc"])
        .Build();

    public void Migrate()
    {
        var appliedMigrations = context.Database.GetAppliedMigrations();

        context.Database.Migrate();

        if (!appliedMigrations.Any())
        {
            Directory.CreateDirectory(s_imagesDirectory);

            User adminUser = new()
            {
                UserName = "admin@example.com",
                Email = "admin@example.com",
                EmailConfirmed = true
            };

            IdentityRole adminRole = new()
            {
                Name = "Administrator"
            };

            var password = configuration.GetValue<string>("AdminPassword") ?? "AdminPass123!";

            var identityResult = userManager.CreateAsync(adminUser, password).Result;
            identityResult = roleManager.CreateAsync(adminRole).Result;
            identityResult = userManager.AddToRoleAsync(adminUser, adminRole.Name).Result;

            var json = File.ReadAllText(s_jsonFile);
            var cuisines = JsonSerializer.Deserialize<Cuisine[]>(json)!;

            foreach (var recipe in cuisines.SelectMany(x => x.Recipes!))
            {
                recipe.Id = idGenerator.CreateId();
                recipe.OwnerId = adminUser.Id;
                recipe.Created = DateTime.UtcNow;
                recipe.Instructions!.Html = Markdown.ToHtml(recipe.Instructions.Markdown!, s_pipeline);

                File.Copy(Path.Combine(s_dataImagesDirectory, recipe.CoverImage!.Url!),
                        Path.Combine(s_imagesDirectory, $"{recipe.Id}.webp"), true);

                recipe.CoverImage.Url = $"{recipe.Id}.webp";
            }

            context.Cuisines.AddRange(cuisines);
            context.SaveChanges();
        }
    }
}
