using JonathanPotts.RecipeCatalog.Domain.Entities;

namespace JonathanPotts.RecipeCatalog.Domain.Services;

public class RecipesRepository(RecipeCatalogDbContext context)
    : BaseRepository<Recipe>(context)
{
}
