using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codeworx.Identity.EntityFrameworkCore.Migrations.Sqlite.Migrations
{
    public partial class RemoveUserRefreshTokenForeignKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRefreshToken_ClientConfiguration_ClientId",
                table: "UserRefreshToken");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRefreshToken_RightHolder_UserId",
                table: "UserRefreshToken");

            migrationBuilder.DropIndex(
                name: "IX_UserRefreshToken_ClientId",
                table: "UserRefreshToken");

            migrationBuilder.DropIndex(
                name: "IX_UserRefreshToken_UserId",
                table: "UserRefreshToken");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserRefreshToken_ClientId",
                table: "UserRefreshToken",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRefreshToken_UserId",
                table: "UserRefreshToken",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRefreshToken_ClientConfiguration_ClientId",
                table: "UserRefreshToken",
                column: "ClientId",
                principalTable: "ClientConfiguration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRefreshToken_RightHolder_UserId",
                table: "UserRefreshToken",
                column: "UserId",
                principalTable: "RightHolder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
