using Microsoft.EntityFrameworkCore.Migrations;

namespace Codeworx.Identity.EntityFrameworkCore.Migrations.Sqlite.Migrations
{
    public partial class AddInvitationAction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE UserInvitation DROP COLUMN CanChangeLogin");

            migrationBuilder.AddColumn<int>(
                name: "Action",
                table: "UserInvitation",
                nullable: false,
                defaultValue: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE UserInvitation DROP COLUMN Action");

            migrationBuilder.AddColumn<bool>(
                name: "CanChangeLogin",
                table: "UserInvitation",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
