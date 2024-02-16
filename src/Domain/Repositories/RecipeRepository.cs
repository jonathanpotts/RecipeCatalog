using JonathanPotts.RecipeCatalog.Domain.Entities;

namespace JonathanPotts.RecipeCatalog.Domain.Repositories;

public class RecipeRepository(RecipeCatalogDbContext context)
    : BaseRepository<Recipe>(context)
{
}
