using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Codeworx.Identity.EntityFrameworkCore.Migrations
{
    public partial class Provider : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExternalAuthenticationProvider",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    EndpointConfiguration = table.Column<string>(nullable: true),
                    EndpointType = table.Column<string>(maxLength: 100, nullable: false),
                    FilterId = table.Column<Guid>(nullable: true),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalAuthenticationProvider", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuthenticationProviderUser",
                columns: table => new
                {
                    RightHolderId = table.Column<Guid>(nullable: false),
                    ProviderId = table.Column<Guid>(nullable: false),
                    ExternalIdentifier = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthenticationProviderUser", x => new { x.RightHolderId, x.ProviderId });
                    table.ForeignKey(
                        name: "FK_AuthenticationProviderUser_ExternalAuthenticationProvider_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "ExternalAuthenticationProvider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuthenticationProviderUser_RightHolder_RightHolderId",
                        column: x => x.RightHolderId,
                        principalTable: "RightHolder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthenticationProviderUser_ProviderId",
                table: "AuthenticationProviderUser",
                column: "ProviderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthenticationProviderUser");

            migrationBuilder.DropTable(
                name: "ExternalAuthenticationProvider");
        }
    }
}
