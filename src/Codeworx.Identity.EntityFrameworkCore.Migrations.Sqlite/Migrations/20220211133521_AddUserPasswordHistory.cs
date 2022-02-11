using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Codeworx.Identity.EntityFrameworkCore.Migrations.Sqlite.Migrations
{
    public partial class AddUserPasswordHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserPasswordHistory",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    PasswordHash = table.Column<string>(maxLength: 512, nullable: false),
                    ChangedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPasswordHistory", x => new { x.UserId, x.PasswordHash });
                    table.ForeignKey(
                        name: "FK_UserPasswordHistory_RightHolder_UserId",
                        column: x => x.UserId,
                        principalTable: "RightHolder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPasswordHistory");
        }
    }
}
