using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codeworx.Identity.EntityFrameworkCore.Migrations.Sqlite.Migrations
{
    public partial class ExtendProviderValueUniqueIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AuthenticationProviderRightHolder_ExternalId_Unique",
                table: "AuthenticationProviderRightHolder");

            migrationBuilder.CreateIndex(
                name: "IX_AuthenticationProviderRightHolder_ExternalId_Unique",
                table: "AuthenticationProviderRightHolder",
                columns: new[] { "ExternalIdentifier", "ProviderId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AuthenticationProviderRightHolder_ExternalId_Unique",
                table: "AuthenticationProviderRightHolder");

            migrationBuilder.CreateIndex(
                name: "IX_AuthenticationProviderRightHolder_ExternalId_Unique",
                table: "AuthenticationProviderRightHolder",
                column: "ExternalIdentifier",
                unique: true);
        }
    }
}
