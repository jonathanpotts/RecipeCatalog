using JonathanPotts.RecipeCatalog.Domain.Entities;

namespace JonathanPotts.RecipeCatalog.Domain.Repositories;

public class CuisineRepository(RecipeCatalogDbContext context)
    : BaseRepository<Cuisine>(context)
{
}
