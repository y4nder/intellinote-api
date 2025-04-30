using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEmbeddingFormat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Summary",
                table: "Notes");

            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "Notes",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Summary",
                table: "Notes");

            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "Notes",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
