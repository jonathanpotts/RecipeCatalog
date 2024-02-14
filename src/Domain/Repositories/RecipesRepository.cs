using JonathanPotts.RecipeCatalog.Domain.Entities;

namespace JonathanPotts.RecipeCatalog.Domain.Repositories;

public class RecipesRepository(RecipeCatalogDbContext context)
    : BaseRepository<Recipe>(context)
{
}
