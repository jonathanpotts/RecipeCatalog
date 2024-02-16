using JonathanPotts.RecipeCatalog.Application.Contracts.Models;
using JonathanPotts.RecipeCatalog.Domain.Entities;

namespace JonathanPotts.RecipeCatalog.Application.Mapping;

public static class CuisineExtensions
{
    public static CuisineDto ToCuisineDto(this Cuisine cuisine)
    {
        return new CuisineDto
        {
            Id = cuisine.Id,
            Name = cuisine.Name,
        };
    }

    public static Cuisine ToCuisine(this CreateUpdateCuisineDto dto)
    {
        return new Cuisine
        {
            Name = dto.Name,
        };
    }
}
