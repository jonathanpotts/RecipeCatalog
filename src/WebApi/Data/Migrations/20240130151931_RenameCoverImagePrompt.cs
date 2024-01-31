using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JonathanPotts.RecipeCatalog.WebApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameCoverImagePrompt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CoverImagePrompt",
                table: "Recipes",
                newName: "CoverImageAltText");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CoverImageAltText",
                table: "Recipes",
                newName: "CoverImagePrompt");
        }
    }
}
