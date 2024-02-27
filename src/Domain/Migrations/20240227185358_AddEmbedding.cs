using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JonathanPotts.RecipeCatalog.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddEmbedding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Embedding",
                table: "Recipes",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Embedding",
                table: "Recipes");
        }
    }
}
