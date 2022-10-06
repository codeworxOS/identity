using Microsoft.EntityFrameworkCore.Migrations;

namespace Codeworx.Identity.EntityFrameworkCore.Migrations.SqlServer.Migrations
{
    public partial class AddMfaEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AuthenticationMode",
                table: "Tenant",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AuthenticationMode",
                table: "RightHolder",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AuthenticationMode",
                table: "ClientConfiguration",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Usage",
                table: "AuthenticationProvider",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthenticationMode",
                table: "Tenant");

            migrationBuilder.DropColumn(
                name: "AuthenticationMode",
                table: "RightHolder");

            migrationBuilder.DropColumn(
                name: "AuthenticationMode",
                table: "ClientConfiguration");

            migrationBuilder.DropColumn(
                name: "Usage",
                table: "AuthenticationProvider");
        }
    }
}
