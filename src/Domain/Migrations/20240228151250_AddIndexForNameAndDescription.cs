using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JonathanPotts.RecipeCatalog.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexForNameAndDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Recipes_Description",
                table: "Recipes",
                column: "Description");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_Name",
                table: "Recipes",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Recipes_Description",
                table: "Recipes");

            migrationBuilder.DropIndex(
                name: "IX_Recipes_Name",
                table: "Recipes");
        }
    }
}
