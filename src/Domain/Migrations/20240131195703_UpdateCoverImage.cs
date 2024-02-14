using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JonathanPotts.RecipeCatalog.Domain.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCoverImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CoverImageAltText",
                table: "Recipes",
                newName: "CoverImage_AltText");

            migrationBuilder.RenameColumn(
                name: "CoverImage",
                table: "Recipes",
                newName: "CoverImage_Url");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CoverImage_Url",
                table: "Recipes",
                newName: "CoverImage");

            migrationBuilder.RenameColumn(
                name: "CoverImage_AltText",
                table: "Recipes",
                newName: "CoverImageAltText");
        }
    }
}
