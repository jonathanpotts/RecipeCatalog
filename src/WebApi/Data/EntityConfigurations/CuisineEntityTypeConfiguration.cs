using JonathanPotts.RecipeCatalog.WebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JonathanPotts.RecipeCatalog.WebApi.Data.EntityConfigurations;

public class CuisineEntityTypeConfiguration : IEntityTypeConfiguration<Cuisine>
{
    public void Configure(EntityTypeBuilder<Cuisine> builder)
    {
        builder.HasIndex(x => x.Name)
            .IsUnique();
    }
}
