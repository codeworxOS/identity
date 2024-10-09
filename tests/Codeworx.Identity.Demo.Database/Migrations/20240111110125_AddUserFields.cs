using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codeworx.Identity.Demo.Database.Migrations
{
    public partial class AddUserFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "RightHolder",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "RightHolder",
                type: "TEXT",
                maxLength: 800,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Organization",
                table: "RightHolder",
                type: "TEXT",
                maxLength: 500,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Department",
                table: "RightHolder");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "RightHolder");

            migrationBuilder.DropColumn(
                name: "Organization",
                table: "RightHolder");
        }
    }
}
