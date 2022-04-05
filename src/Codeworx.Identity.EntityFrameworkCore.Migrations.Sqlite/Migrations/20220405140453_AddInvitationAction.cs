using Microsoft.EntityFrameworkCore.Migrations;

namespace Codeworx.Identity.EntityFrameworkCore.Migrations.Sqlite.Migrations
{
    public partial class AddInvitationAction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanChangeLogin",
                table: "UserInvitation");

            migrationBuilder.AddColumn<int>(
                name: "Action",
                table: "UserInvitation",
                nullable: false,
                defaultValue: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Action",
                table: "UserInvitation");

            migrationBuilder.AddColumn<bool>(
                name: "CanChangeLogin",
                table: "UserInvitation",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
