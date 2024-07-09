using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipeCatalog.Domain.Migrations
{
    /// <inheritdoc />
    public partial class RenameEmbedding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Embedding",
                table: "Recipes",
                newName: "Embeddings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Embeddings",
                table: "Recipes",
                newName: "Embedding");
        }
    }
}
