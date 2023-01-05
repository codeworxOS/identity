using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codeworx.Identity.EntityFrameworkCore.Migrations.Sqlite.Migrations
{
    public partial class RedesignTokenCache : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "IdentityCache",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccessTokenType",
                table: "ClientConfiguration",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccessTokenTypeConfiguration",
                table: "ClientConfiguration",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Usage",
                table: "AuthenticationProvider",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldDefaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_IdentityCache_UserId",
                table: "IdentityCache",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_IdentityCache_RightHolder_UserId",
                table: "IdentityCache",
                column: "UserId",
                principalTable: "RightHolder",
                principalColumn: "Id");

            migrationBuilder.Sql(@"INSERT INTO IdentityCache (Key, Value, UserId,ValidUntil, CacheType, Disabled)
SELECT Token, IdentityData, UserId, ValidUntil, 4,IsDisabled FROM UserRefreshToken");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdentityCache_RightHolder_UserId",
                table: "IdentityCache");

            migrationBuilder.DropIndex(
                name: "IX_IdentityCache_UserId",
                table: "IdentityCache");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "IdentityCache");

            migrationBuilder.DropColumn(
                name: "AccessTokenType",
                table: "ClientConfiguration");

            migrationBuilder.DropColumn(
                name: "AccessTokenTypeConfiguration",
                table: "ClientConfiguration");

            migrationBuilder.AlterColumn<int>(
                name: "Usage",
                table: "AuthenticationProvider",
                type: "INTEGER",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldDefaultValue: 0);

        }
    }
}
