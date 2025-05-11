using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update_Note_Gin_Index : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Notes_NormalizedContent",
                table: "Notes");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_Title_Summary_NormalizedContent",
                table: "Notes",
                columns: new[] { "Title", "Summary", "NormalizedContent" })
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:TsVectorConfig", "english");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Notes_Title_Summary_NormalizedContent",
                table: "Notes");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_NormalizedContent",
                table: "Notes",
                column: "NormalizedContent")
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:TsVectorConfig", "english");
        }
    }
}
