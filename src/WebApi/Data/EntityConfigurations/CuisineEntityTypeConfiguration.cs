using JonathanPotts.RecipeBook.WebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JonathanPotts.RecipeBook.WebApi.Data.EntityConfigurations;

public class CuisineEntityTypeConfiguration : IEntityTypeConfiguration<Cuisine>
{
    public void Configure(EntityTypeBuilder<Cuisine> builder)
    {
        builder.HasIndex(x => x.Name)
            .IsUnique();
    }
}
