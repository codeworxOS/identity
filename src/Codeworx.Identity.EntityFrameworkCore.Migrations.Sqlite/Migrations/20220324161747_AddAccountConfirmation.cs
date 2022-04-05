using Microsoft.EntityFrameworkCore.Migrations;

namespace Codeworx.Identity.EntityFrameworkCore.Migrations.Sqlite.Migrations
{
    public partial class AddAccountConfirmation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConfirmationCode",
                table: "RightHolder",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ConfirmationPending",
                table: "RightHolder",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RightHolder_Name_Unique",
                table: "RightHolder",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RightHolder_Name_Unique",
                table: "RightHolder");

            migrationBuilder.DropColumn(
                name: "ConfirmationCode",
                table: "RightHolder");

            migrationBuilder.DropColumn(
                name: "ConfirmationPending",
                table: "RightHolder");
        }
    }
}
