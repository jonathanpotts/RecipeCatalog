using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JonathanPotts.RecipeCatalog.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddEmbeddingsForNameAndDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Embeddings",
                table: "Recipes",
                newName: "NameEmbeddings");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionEmbeddings",
                table: "Recipes",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionEmbeddings",
                table: "Recipes");

            migrationBuilder.RenameColumn(
                name: "NameEmbeddings",
                table: "Recipes",
                newName: "Embeddings");
        }
    }
}
