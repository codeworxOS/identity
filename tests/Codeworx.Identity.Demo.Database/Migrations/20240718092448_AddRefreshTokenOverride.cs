using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codeworx.Identity.Demo.Database.Migrations
{
    public partial class AddRefreshTokenOverride : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RefreshTokenExpiration",
                table: "ClientConfiguration",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RefreshTokenLifetime",
                table: "ClientConfiguration",
                type: "INTEGER",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiration",
                table: "ClientConfiguration");

            migrationBuilder.DropColumn(
                name: "RefreshTokenLifetime",
                table: "ClientConfiguration");
        }
    }
}
