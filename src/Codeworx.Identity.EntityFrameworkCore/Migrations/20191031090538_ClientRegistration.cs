using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Codeworx.Identity.EntityFrameworkCore.Migrations
{
    public partial class ClientRegistration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClientConfiguration",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ClientSecret = table.Column<byte[]>(nullable: true),
                    ClientSecretSalt = table.Column<byte[]>(nullable: true),
                    DefaultRedirectUri = table.Column<string>(nullable: true),
                    FlowTypes = table.Column<int>(nullable: false),
                    TokenExpiration = table.Column<TimeSpan>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientConfiguration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ValidRedirectUrl",
                columns: table => new
                                  {
                                      Id = table.Column<Guid>(nullable: false),
                                      ClientConfigurationId = table.Column<Guid>(nullable: true),
                                      Url = table.Column<string>(nullable: true)
                                  },
                constraints: table =>
                             {
                                 table.PrimaryKey("PK_ValidRedirectUrl", x => x.Id);
                                 table.ForeignKey(
                                     name: "FK_ValidRedirectUrl_ClientConfiguration_ClientConfigurationId",
                                     column: x => x.ClientConfigurationId,
                                     principalTable: "ClientConfiguration",
                                     principalColumn: "Id",
                                     onDelete: ReferentialAction.Restrict);
                             });

            migrationBuilder.CreateIndex(
                name: "IX_ValidRedirectUrl_ClientConfigurationId",
                table: "ValidRedirectUrl",
                column: "ClientConfigurationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientConfiguration");

            migrationBuilder.DropTable(
                name: "ValidRedirectUrl");
        }
    }
}