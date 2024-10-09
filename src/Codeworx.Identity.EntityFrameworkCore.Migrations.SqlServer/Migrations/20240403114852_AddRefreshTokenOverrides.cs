using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codeworx.Identity.EntityFrameworkCore.Migrations.SqlServer.Migrations
{
    public partial class AddRefreshTokenOverrides : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RefreshTokenExpiration",
                table: "ClientConfiguration",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RefreshTokenLifetime",
                table: "ClientConfiguration",
                type: "int",
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
