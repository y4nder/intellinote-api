using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace WebApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class Added_Embeddings_Folder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Keywords",
                table: "Folders",
                type: "text",
                nullable: false,
                oldClrType: typeof(List<string>),
                oldType: "text[]");

            migrationBuilder.AddColumn<Vector>(
                name: "Embedding",
                table: "Folders",
                type: "vector(1536)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Folders_Embedding",
                table: "Folders",
                column: "Embedding")
                .Annotation("Npgsql:IndexMethod", "ivfflat")
                .Annotation("Npgsql:IndexOperators", new[] { "vector_cosine_ops" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Folders_Embedding",
                table: "Folders");

            migrationBuilder.DropColumn(
                name: "Embedding",
                table: "Folders");

            migrationBuilder.AlterColumn<List<string>>(
                name: "Keywords",
                table: "Folders",
                type: "text[]",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
