﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RecipeCatalog.Domain;

#nullable disable

namespace RecipeCatalog.Domain.Migrations
{
    [DbContext(typeof(RecipeCatalogDbContext))]
    [Migration("20240119160705_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.1");

            modelBuilder.Entity("RecipeCatalog.WebApi.Models.Cuisine", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Cuisines");
                });

            modelBuilder.Entity("RecipeCatalog.WebApi.Models.Recipe", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Created")
                        .HasColumnType("TEXT");

                    b.Property<int>("CuisineId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Ingredients")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("Modified")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CuisineId");

                    b.ToTable("Recipes");
                });

            modelBuilder.Entity("RecipeCatalog.WebApi.Models.Recipe", b =>
                {
                    b.HasOne("RecipeCatalog.WebApi.Models.Cuisine", "Cuisine")
                        .WithMany("Recipes")
                        .HasForeignKey("CuisineId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("RecipeCatalog.WebApi.Models.MarkdownData", "Instructions", b1 =>
                        {
                            b1.Property<long>("RecipeId")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("Html")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.Property<string>("Markdown")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.HasKey("RecipeId");

                            b1.ToTable("Recipes");

                            b1.WithOwner()
                                .HasForeignKey("RecipeId");
                        });

                    b.Navigation("Cuisine");

                    b.Navigation("Instructions")
                        .IsRequired();
                });

            modelBuilder.Entity("RecipeCatalog.WebApi.Models.Cuisine", b =>
                {
                    b.Navigation("Recipes");
                });
#pragma warning restore 612, 618
        }
    }
}
