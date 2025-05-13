using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class added_views_nasad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_View_AspNetUsers_UserId",
                table: "View");

            migrationBuilder.DropForeignKey(
                name: "FK_View_UserData_UserDataId",
                table: "View");

            migrationBuilder.DropPrimaryKey(
                name: "PK_View",
                table: "View");

            migrationBuilder.RenameTable(
                name: "View",
                newName: "Views");

            migrationBuilder.RenameIndex(
                name: "IX_View_UserId",
                table: "Views",
                newName: "IX_Views_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_View_UserDataId",
                table: "Views",
                newName: "IX_Views_UserDataId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Views",
                table: "Views",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Views_AspNetUsers_UserId",
                table: "Views",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Views_UserData_UserDataId",
                table: "Views",
                column: "UserDataId",
                principalTable: "UserData",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Views_AspNetUsers_UserId",
                table: "Views");

            migrationBuilder.DropForeignKey(
                name: "FK_Views_UserData_UserDataId",
                table: "Views");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Views",
                table: "Views");

            migrationBuilder.RenameTable(
                name: "Views",
                newName: "View");

            migrationBuilder.RenameIndex(
                name: "IX_Views_UserId",
                table: "View",
                newName: "IX_View_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Views_UserDataId",
                table: "View",
                newName: "IX_View_UserDataId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_View",
                table: "View",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_View_AspNetUsers_UserId",
                table: "View",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_View_UserData_UserDataId",
                table: "View",
                column: "UserDataId",
                principalTable: "UserData",
                principalColumn: "Id");
        }
    }
}
