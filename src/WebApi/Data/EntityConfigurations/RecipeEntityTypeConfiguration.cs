using JonathanPotts.RecipeCatalog.WebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace JonathanPotts.RecipeCatalog.WebApi.Data.EntityConfigurations;

public class RecipeEntityTypeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        ValueConverter<DateTime, DateTime> utcConverter = new(
            x => x.ToUniversalTime(),
            x => DateTime.SpecifyKind(x, DateTimeKind.Utc));

        ValueConverter<DateTime?, DateTime?> nullableUtcConverter = new(
            x => x != null ? utcConverter.ConvertToProviderTyped(x.Value) : null,
            x => x != null ? utcConverter.ConvertFromProviderTyped(x.Value) : null);

        builder.OwnsOne(x => x.CoverImage);

        builder.OwnsOne(x => x.Instructions);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Created)
            .HasConversion(utcConverter);

        builder.Property(x => x.Modified)
            .HasConversion(nullableUtcConverter);
    }
}
