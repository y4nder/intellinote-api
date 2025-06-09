using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update_userdata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Folders_UserData_UserDataId",
                table: "Folders");

            migrationBuilder.DropForeignKey(
                name: "FK_Notes_UserData_UserDataId",
                table: "Notes");

            migrationBuilder.DropForeignKey(
                name: "FK_Views_UserData_UserDataId",
                table: "Views");

            migrationBuilder.DropIndex(
                name: "IX_Views_UserDataId",
                table: "Views");

            migrationBuilder.DropIndex(
                name: "IX_Notes_UserDataId",
                table: "Notes");

            migrationBuilder.DropIndex(
                name: "IX_Folders_UserDataId",
                table: "Folders");

            migrationBuilder.DropColumn(
                name: "UserDataId",
                table: "Views");

            migrationBuilder.DropColumn(
                name: "UserDataId",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "UserDataId",
                table: "Folders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserDataId",
                table: "Views",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserDataId",
                table: "Notes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserDataId",
                table: "Folders",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Views_UserDataId",
                table: "Views",
                column: "UserDataId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_UserDataId",
                table: "Notes",
                column: "UserDataId");

            migrationBuilder.CreateIndex(
                name: "IX_Folders_UserDataId",
                table: "Folders",
                column: "UserDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_Folders_UserData_UserDataId",
                table: "Folders",
                column: "UserDataId",
                principalTable: "UserData",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_UserData_UserDataId",
                table: "Notes",
                column: "UserDataId",
                principalTable: "UserData",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Views_UserData_UserDataId",
                table: "Views",
                column: "UserDataId",
                principalTable: "UserData",
                principalColumn: "Id");
        }
    }
}
