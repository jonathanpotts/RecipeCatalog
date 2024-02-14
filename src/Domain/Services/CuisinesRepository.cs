using JonathanPotts.RecipeCatalog.Domain.Entities;

namespace JonathanPotts.RecipeCatalog.Domain.Services;

public class CuisinesRepository(RecipeCatalogDbContext context)
    : BaseRepository<Cuisine>(context)
{
}
