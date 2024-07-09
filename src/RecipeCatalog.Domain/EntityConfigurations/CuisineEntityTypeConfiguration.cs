using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecipeCatalog.Domain.Entities;

namespace RecipeCatalog.Domain.EntityConfigurations;

public class CuisineEntityTypeConfiguration : IEntityTypeConfiguration<Cuisine>
{
    public void Configure(EntityTypeBuilder<Cuisine> builder)
    {
        builder.Property(x => x.Name)
            .IsRequired();

        builder.HasIndex(x => x.Name)
            .IsUnique();
    }
}
