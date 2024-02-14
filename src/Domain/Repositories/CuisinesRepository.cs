using JonathanPotts.RecipeCatalog.Domain.Entities;

namespace JonathanPotts.RecipeCatalog.Domain.Repositories;

public class CuisinesRepository(RecipeCatalogDbContext context)
    : BaseRepository<Cuisine>(context)
{
}
